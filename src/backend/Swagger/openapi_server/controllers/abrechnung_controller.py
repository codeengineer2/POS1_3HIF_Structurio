## @file abrechnung_controller.py
#  @brief API-Endpunkte für die Verwaltung von Abrechnungen.
#  @details Enthält Funktionen zur Erstellung und Abfrage von Abrechnungsdatensätzen aus einer PostgreSQL-Datenbank.

## @package abrechnung_controller
#  @brief Modul zur Verwaltung von Abrechnungen in einem Projekt.


import psycopg2
import psycopg2.extras
from flask import jsonify, request, abort
from datetime import date, time
import logging
logger = logging.getLogger(__name__)
## @brief Stellt eine Verbindung zur PostgreSQL-Datenbank her.
#  @return psycopg2-Verbindung
def get_connection():
    """
    Stellt eine Verbindung zur Neon-Postgres-Datenbank her.
    """
    logger.debug("Öffne DB-Verbindung (Abrechnung)")
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
    logger.info("get_abrechnungen aufgerufen", extra={"uid": uid, "pid": pid})
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
        logger.debug("Abrechnungen abgerufen", extra={"count": len(rows)})
    except Exception as e:
        logger.exception("Fehler beim Abrufen der Abrechnungen")
        abort(500, description="Datenbankfehler")
    finally:
        cur.close()
        conn.close()

    if not rows:
        logger.warning("Keine Abrechnungen gefunden", extra={"uid": uid, "pid": pid})
        abort(404, description="Keine Abrechnungen für Benutzer und Projekt gefunden")

    return jsonify([_serialize_record(r) for r in rows]), 200

## @brief Erstellt einen neuen Abrechnungseintrag.
#  @return JSON-Antwort mit dem neu erstellten Abrechnungseintrag oder Fehlermeldung.
def create_abrechnung():
    logger.info("create_abrechnung aufgerufen")
    data = request.get_json(silent=True)
    if not data:
        logger.warning("Kein JSON-Body")
        abort(400, description="JSON-Body fehlt oder ist ungültig")


    required = ["uid", "pid", "name", "date", "price", "category", "rechnung"]

    missing = [f for f in required if f not in data]
    if missing:
        logger.warning("Fehlende Felder beim Erstellen", extra={"missing": missing})
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
        logger.debug("Abrechnung erstellt", extra={"aid": record["aid"]})
    except Exception as e:
        logger.exception("Fehler beim Erstellen der Abrechnung")
        conn.rollback()
        abort(500, description=f"DB-Fehler: {e}")
    finally:
        conn.close()

    return jsonify(_serialize_record(record)), 201