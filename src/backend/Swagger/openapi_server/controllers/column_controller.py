import psycopg2
import logging
from flask import request, jsonify, abort
from openapi_server.models.add_column_request import AddColumnRequest
from openapi_server.models.update_column_request import UpdateColumnRequest

logging.basicConfig(level=logging.INFO)

def get_connection():
    """
    @brief Stellt eine Verbindung zur Neon-Datenbank her.
    """
    logging.info("Verbindung zur Datenbank wird aufgebaut.")
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def add_column(body):
    """
    @brief Erstellt eine neue Spalte.

    @param body: JSON mit board_id und name
    @return: JSON mit neuer Column-ID und Name
    """
    logging.info("Empfange Anfrage zum Erstellen einer Spalte.")
    board_id = body.get("board_id")
    name = body.get("name")

    if not board_id or not name:
        logging.warning("board_id oder name fehlt.")
        abort(400, "board_id und name sind erforderlich.")

    conn = get_connection()
    
    try:
        cur = conn.cursor()
        logging.info("SQL-Befehl zum Einfügen wird ausgeführt.")

        cur.execute("INSERT INTO columns (bid, name) VALUES (%s, %s) RETURNING cid", (board_id, name))
        cid = cur.fetchone()[0]

        conn.commit()
        logging.info("Spalte erfolgreich eingefügt.")
        return jsonify({"cid": cid, "name": name}), 201
    
    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Erstellen: {str(e)}")
        abort(500, f"Fehler: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")

def update_column(body):
    """
    @brief Aktualisiert den Namen einer Spalte.

    @param body: JSON mit cid und name
    @return: JSON mit Erfolgsmeldung
    """
    logging.info("Empfange Anfrage zum Aktualisieren einer Spalte.")
    cid = body.get("id")
    name = body.get("name")

    if not cid or not name:
        logging.warning("id oder name fehlt.")
        abort(400, "id und name sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info(f"Aktualisiere Spalte mit ID {cid}.")

        cur.execute("UPDATE columns SET name = %s WHERE cid = %s", (name, cid))
        if cur.rowcount == 0:
            logging.warning("Spalte nicht gefunden.")
            abort(404, "Spalte nicht gefunden.")

        conn.commit()
        logging.info("Spalte erfolgreich aktualisiert.")
        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Aktualisieren: {str(e)}")
        abort(500, f"Fehler: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")