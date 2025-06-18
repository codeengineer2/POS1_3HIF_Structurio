import psycopg2
import logging
from flask import request, jsonify, abort
from openapi_server.models.project_request import ProjectRequest
from openapi_server.models.update_project_request import UpdateProjectRequest

logging.basicConfig(level=logging.INFO)

def get_connection():
    """
    @brief Stellt eine Verbindung zur Neon-Datenbank her.
    """
    logging.info("Verbindung zur Datenbank wird hergestellt.")
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def create_project(body):
    """
    @brief Erstellt ein neues Projekt und ein leeres Board.

    @param body: JSON mit name, description, color und owner_uid
    @return: JSON mit Erfolgsmeldung, pid und board-ID
    """
    logging.info("Empfange Anfrage zum Erstellen eines Projekts.")
    name = body.get("name")
    description = body.get("description")
    color = body.get("color")
    owner_uid = body.get("owner_uid")

    if name is None or color is None or owner_uid is None:
        logging.warning("Erforderliche Felder fehlen beim Erstellen.")
        abort(400, "Felder name, color und owner_uid sind erforderlich")

    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info("Füge Projekt in die Datenbank ein.")

        cur.execute("""
            INSERT INTO projects (name, description, color, owner_uid)
            VALUES (%s, %s, %s, %s)
            RETURNING pid
        """, (name, description, color, owner_uid))
        pid = cur.fetchone()[0]

        logging.info("Erstelle leeres Board für das Projekt.")

        cur.execute("INSERT INTO boards (pid) VALUES (%s) RETURNING bid", (pid,))
        bid = cur.fetchone()[0]

        conn.commit()
        logging.info("Projekt und Board erfolgreich erstellt.")

        return jsonify({
            "success": True,
            "pid": pid,
            "board": {
                "id": bid,
                "project_id": pid
            }
        }), 201

    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Erstellen: {str(e)}")
        abort(500, f"Fehler beim Anlegen des Projekts: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")

def update_project(body):
    """
    @brief Aktualisiert die Attribute eines Projekts.

    @param body: JSON mit pid, name, description, color
    @return: JSON mit Erfolgsmeldung
    """
    logging.info("Empfange Anfrage zum Aktualisieren eines Projekts.")
    pid = body.get("pid")
    name = body.get("name")
    description = body.get("description")
    color = body.get("color")

    if not pid or not name or not color:
        logging.warning("Erforderliche Felder fehlen beim Aktualisieren.")
        abort(400, "Felder pid, name und color sind erforderlich")

    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info(f"Prüfe ob Projekt mit ID {pid} existiert.")

        cur.execute("SELECT pid FROM projects WHERE pid = %s", (pid,))
        if not cur.fetchone():
            logging.warning("Projekt wurde nicht gefunden.")
            abort(404, "Projekt nicht gefunden")

        logging.info("Aktualisiere Projektdaten.")

        cur.execute("""
            UPDATE projects
            SET name = %s, description = %s, color = %s
            WHERE pid = %s
        """, (name, description, color, pid))

        conn.commit()
        logging.info("Projekt erfolgreich aktualisiert.")
        return jsonify({"success": True}), 200

    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Aktualisieren: {str(e)}")
        abort(500, f"Fehler beim Aktualisieren: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")

def delete_project(pid):
    """
    @brief Löscht ein Projekt.

    @param pid: Die Project-ID
    @return: JSON mit Erfolgsmeldung
    """
    logging.info(f"Empfange Anfrage zum Löschen des Projekts mit ID {pid}.")
    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info("Prüfe ob Projekt existiert.")

        cur.execute("SELECT pid FROM projects WHERE pid = %s", (pid,))
        if not cur.fetchone():
            logging.warning("Projekt wurde nicht gefunden.")
            abort(404, "Projekt nicht gefunden")

        logging.info("Lösche Projekt.")

        cur.execute("DELETE FROM projects WHERE pid = %s", (pid,))

        conn.commit()
        logging.info("Projekt erfolgreich gelöscht.")
        return jsonify({"success": True}), 200

    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Löschen: {str(e)}")
        abort(500, f"Fehler beim Löschen: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")