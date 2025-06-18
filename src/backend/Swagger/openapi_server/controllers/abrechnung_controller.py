## @file abrechnung_controller.py
#  @brief API-Endpunkte für die Verwaltung von Abrechnungen.
#  @details Enthält Funktionen zur Erstellung und Abfrage von Abrechnungsdatensätzen aus einer PostgreSQL-Datenbank.

## @package abrechnung_controller
#  @brief Modul zur Verwaltung von Abrechnungen in einem Projekt.


import psycopg2
import psycopg2.extras
from flask import jsonify, request, abort
from datetime import date, time

## @brief Stellt eine Verbindung zur PostgreSQL-Datenbank her.
#  @return psycopg2-Verbindung
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

## @brief Serialisiert ein Datenbank-Record-Dictionary.
#  @param record Dictionary mit Datenbankwerten.
#  @return Serialisiertes Dictionary mit Datum/Zeit als String.
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

## @brief Holt alle Abrechnungen für einen Benutzer und ein Projekt.
#  @param uid Benutzer-ID.
#  @param pid Projekt-ID.
#  @return JSON-Antwort mit Abrechnungsdaten oder 404 bei Fehler.
def get_abrechnungen(uid, pid):
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            SELECT aid, name, date, price, category, rechnung
            FROM abrechnung WHERE uid = %s AND pid = %s
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

## @brief Erstellt einen neuen Abrechnungseintrag.
#  @return JSON-Antwort mit dem neu erstellten Abrechnungseintrag oder Fehlermeldung.
def create_abrechnung():

    data = request.get_json(silent=True)
    if not data:
        abort(400, description="JSON-Body fehlt oder ist ungültig")


    required = ["uid", "pid", "name", "date", "price", "category", "rechnung"]

    missing = [f for f in required if f not in data]
    if missing:
        abort(400, description=f"Fehlende Felder: {', '.join(missing)}")



    conn = get_connection()
    try:
        with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cur:
            cur.execute(
                """
                INSERT INTO abrechnung (uid, pid, name, date, price, category, rechnung)
                VALUES (%s, %s, %s, %s, %s, %s, %s)
                RETURNING aid, uid, pid, name, date, price, category, rechnung
                """,
                (
                    data["uid"],
                    data["pid"],
                    data["name"],
                    data["date"],  
                    data["price"],
                    data["category"],
                    data["rechnung"],
                ),
            )
            record = cur.fetchone()
        conn.commit()
    except Exception as e:
        conn.rollback()
        abort(500, description=f"DB-Fehler: {e}")
    finally:
        conn.close()

    return jsonify(_serialize_record(record)), 201