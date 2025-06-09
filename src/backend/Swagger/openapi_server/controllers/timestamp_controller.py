import psycopg2
import psycopg2.extras
from flask import jsonify, request, abort
from datetime import date, time


def get_connection():
    """
    Stellt eine Verbindung zur Neon-Postgres-Datenbank her.
    """
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)


def _serialize_timestamp_record(record: dict) -> dict:
   
    serialized = {}
    for key, value in record.items():
        if isinstance(value, date):
            serialized[key] = value.isoformat()
        elif isinstance(value, time):
            serialized[key] = value.strftime("%H:%M:%S")
        else:
            serialized[key] = value
    return serialized


def get_timestamps_by_user_project(uid, pid):
  
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
        """
        SELECT zid, datum_in, checkin, datum_out, checkout, duration
        FROM zeitstempel
        WHERE uid = %s AND pid = %s
        ORDER BY datum_in, checkin
        """,
        (uid, pid)
        )

        rows = cur.fetchall()
        cur.close()
    finally:
        conn.close()

    if not rows:
        abort(404, description="Keine Zeitstempel gefunden für Benutzer und Projekt")

    serialized = [_serialize_timestamp_record(r) for r in rows]
    return jsonify(serialized), 200


def create_timestamp():

    data = request.get_json(force=True)
    required = ['uid', 'pid', 'datum_in', 'datum_out', 'checkin', 'checkout', 'duration']
    for field in required:
        if field not in data:
            abort(400, description=f"'{field}' ist erforderlich")

    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            INSERT INTO zeitstempel (uid, pid, checkin, checkout, datum_out, datum_in, duration)
            VALUES (%s, %s, %s, %s, %s, %s, %s)
            RETURNING zid, uid, pid, checkin, checkout, datum_out, datum_in, duration
            """,
            (
                data['uid'],
                data['pid'],
                data['checkin'],
                data['checkout'],
                data['datum_out'],
                data['datum_in'],
                data['duration'],
            )
        )
        row = cur.fetchone()
        conn.commit()
        cur.close()
    finally:
        conn.close()

    serialized = _serialize_timestamp_record(row)
    return jsonify(serialized), 201


def get_timestamp_by_id(zid):
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            SELECT zid, uid, pid, checkin, checkout, datum_out, datum_in, duration
            FROM zeitstempel
            WHERE zid = %s
            """,
            (zid,)
        )
        row = cur.fetchone()
        cur.close()
    finally:
        conn.close()

    if not row:
        abort(404, description="Zeitstempel nicht gefunden")

    serialized = _serialize_timestamp_record(row)
    return jsonify(serialized), 200


def update_timestamp(uid, pid, zid):
    data = request.get_json(force=True)
    for field in ('datum_in','checkin','datum_out','checkout','duration'):
        if field not in data:
            abort(400, f"'{field}' ist erforderlich")

    conn = get_connection()
    cur  = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
    updated = cur.execute(
        """
        UPDATE zeitstempel
        SET datum_in=%s, checkin=%s, datum_out=%s, checkout=%s, duration=%s
        WHERE uid=%s AND pid=%s AND zid=%s
        RETURNING zid, uid, pid, datum_in, checkin, datum_out, checkout, duration
        """,
        (data['datum_in'], data['checkin'],
         data['datum_out'], data['checkout'],
         data['duration'], uid, pid, zid)
    )
    row = cur.fetchone()
    conn.commit()
    cur.close()
    conn.close()

    if not row:
        abort(404, "Nicht gefunden")
    return jsonify(_serialize_timestamp_record(row)), 200

