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


def _serialize_record(record: dict) -> dict:
   
    serialized = {}
    for key, value in record.items():
        if isinstance(value, date):
            serialized[key] = value.isoformat()
        elif isinstance(value, time):
            serialized[key] = value.strftime("%H:%M:%S")
        else:
            serialized[key] = value
    return serialized


def get_abrechnungen(uid, pid):
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            SELECT aid, name, date, price, category, rechnung
            FROM abrechnung WHERE pid = %s AND uid = %s
            ORDER BY date
            """,
            (uid, pid)
        )
        rows = cur.fetchall()
        cur.close()
    finally:
        conn.close()
    if not rows:
        abort(404, description="Keine Zeitstempel gefunden für Benutzer und Projekt")
    serialized= [_serialize_record(r) for r in rows]
    return jsonify(serialized), 200

def create_abrechnungen():
    pass