## @file timestamp_controller.py
#  @brief API-Endpunkte für die Verwaltung von Zeitstempeln.
#  @details Dieses Modul verwaltet Zeitstempel für Benutzeraktivitäten in Projekten.

## @package timestamp_controller
#  @brief Modul zur Verwaltung von Zeitstempeldaten in einer PostgreSQL-Datenbank.
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
    logger.debug("Öffne DB-Verbindung")
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

## @brief Serialisiert einen Zeitstempel-Datensatz.
#  @param record Dictionary mit Datenbankwerten.
#  @return Serialisiertes Dictionary mit Datum/Zeit als String.
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

## @brief Gibt alle Zeitstempel eines Benutzers zurück.
#  @param uid Benutzer-ID.
#  @return JSON-Antwort mit Zeitstempeldaten oder 404 bei Fehler.
def get_timestamps_by_user(uid):
    logger.info("get_timestamps_by_user aufgerufen", extra={"uid": uid})
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            SELECT zid, datum_in, checkin, datum_out, checkout, duration
            FROM zeitstempel
            WHERE uid = %s
            ORDER BY datum_in, checkin
            """,
            (uid,)
        )
        rows = cur.fetchall()
        logger.debug("Datensätze abgerufen", extra={"count": len(rows)})
    except Exception as e:
        logger.exception("Fehler beim Abrufen der Zeitstempel")
        abort(500, description="Datenbankfehler")
    finally:
        cur.close()
        conn.close()

    if not rows:
        logger.warning("Keine Zeitstempel gefunden", extra={"uid": uid})
        abort(404, description="Keine Zeitstempel für diesen Benutzer gefunden")

    result = [_serialize_timestamp_record(r) for r in rows]
    logger.info("Rückgabe Zeitstempel", extra={"returned": len(result)})
    return jsonify(result), 200

## @brief Erstellt einen neuen Zeitstempel.
#  @return JSON-Antwort mit dem neu erstellten Zeitstempel oder Fehlermeldung.
def create_timestamp():
    logger.info("create_timestamp aufgerufen")
    data = request.get_json(force=True)
    required = ['uid', 'datum_in', 'datum_out', 'checkin', 'checkout', 'duration']
    for field in required:
        if field not in data:
            logger.warning("Fehlendes Feld", extra={"field": field})
            abort(400, description=f"'{field}' ist erforderlich")

    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            INSERT INTO zeitstempel (uid, checkin, checkout, datum_out, datum_in, duration)
            VALUES (%s, %s, %s, %s, %s, %s)
            RETURNING zid, uid, checkin, checkout, datum_out, datum_in, duration
            """,
            (
                data['uid'],
                data['checkin'],
                data['checkout'],
                data['datum_out'],
                data['datum_in'],
                data['duration'],
            )
        )
        row = cur.fetchone()
        conn.commit()
        logger.debug("Zeitstempel erstellt", extra={"zid": row['zid'], "uid": row['uid']})
    except Exception as e:
        logger.exception("Fehler beim Erstellen des Zeitstempels")
        conn.rollback()
        abort(500, description="Datenbankfehler")
    finally:
        cur.close()
        conn.close()

    serialized = _serialize_timestamp_record(row)
    return jsonify(serialized), 201

## @brief Gibt einen Zeitstempel anhand der ID zurück.
#  @param zid Zeitstempel-ID.
#  @return JSON-Antwort mit Zeitstempeldaten oder 404 bei Fehler.
def get_timestamp_by_id(zid):
    logger.info("get_timestamp_by_id aufgerufen", extra={"zid": zid})
    conn = get_connection()
    try:
        cur = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            SELECT zid, uid, checkin, checkout, datum_out, datum_in, duration
            FROM zeitstempel
            WHERE zid = %s
            """,
            (zid,)
        )
        row = cur.fetchone()
        logger.debug("Datensatz abgerufen", extra={"found": bool(row)})
    except Exception as e:
        logger.exception("Fehler beim Abrufen des Zeitstempels")
        abort(500, description="Datenbankfehler")
    finally:
        cur.close()
        conn.close()

    if not row:
        logger.warning("Zeitstempel nicht gefunden", extra={"zid": zid})
        abort(404, description="Zeitstempel nicht gefunden")

    serialized = _serialize_timestamp_record(row)
    return jsonify(serialized), 200

## @brief Aktualisiert einen bestehenden Zeitstempel.
#  @param uid Benutzer-ID.
#  @param zid Zeitstempel-ID.
#  @return JSON-Antwort mit aktualisierten Daten oder 404 bei Fehler.
def update_timestamp(uid, zid):
    logger.info("update_timestamp aufgerufen", extra={"uid": uid, "zid": zid})
    data = request.get_json(force=True)
    for field in ('datum_in','checkin','datum_out','checkout','duration'):
        if field not in data:
            logger.warning("Fehlendes Feld", extra={"field": field})
            abort(400, f"'{field}' ist erforderlich")

    conn = get_connection()
    try:
        cur  = conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor)
        cur.execute(
            """
            UPDATE zeitstempel
            SET datum_in=%s, checkin=%s, datum_out=%s, checkout=%s, duration=%s
            WHERE uid=%s AND zid=%s
            RETURNING zid, uid, datum_in, checkin, datum_out, checkout, duration
            """,
            (data['datum_in'], data['checkin'], data['datum_out'],
             data['checkout'], data['duration'], uid, zid)
        )
        row = cur.fetchone()
        conn.commit()
        logger.debug("Zeitstempel aktualisiert", extra={"zid": zid})
    except Exception as e:
        logger.exception("Fehler beim Aktualisieren des Zeitstempels")
        conn.rollback()
        abort(500, description="Datenbankfehler")
    finally:
        cur.close()
        conn.close()

    if not row:
        logger.warning("Zu aktualisierender Zeitstempel nicht gefunden", extra={"uid": uid, "zid": zid})
        abort(404, description="Nicht gefunden")

    return jsonify(_serialize_timestamp_record(row)), 200

