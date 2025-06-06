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
    #Date und Time Objekte werden hier kovertiert da es ja mit dem automatischen nicht funktioniert.
    serialized = {}

    for key, value in record.items():
        if isinstance(value, date):
            # Datum ins ISO-Format: "YYYY-MM-DD"
            serialized[key] = value.isoformat()
        elif isinstance(value, time):
            # Uhrzeit ins ISO-Format: "HH:MM:SS"
            serialized[key] = value.strftime("%H:%M:%S")
        else:
            serialized[key] = value

    return serialized


def get_all_timestamps():

    conn = get_connection()
    try:
        cursor = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cursor.execute("""
            SELECT
                zid,
                uid,
                pid,
                checkin   AS checkin,
                checkout  AS checkout,
                datum_out AS datum_out,
                datum_in  AS datum_in,
                duration
            FROM zeitstempel
            ORDER BY zid;
        """)
        rows = cursor.fetchall()
        cursor.close()
    finally:
        conn.close()

    
    serialized_list = [ _serialize_timestamp_record(r) for r in rows ]
    return jsonify(serialized_list), 200


def create_timestamp():

    pass

def get_timestamp_by_id(zid):

    pass

def update_timestamp(zid):
 
    pass