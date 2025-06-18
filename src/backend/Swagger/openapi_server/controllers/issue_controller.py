import psycopg2
import logging
from flask import request, jsonify, abort
from openapi_server.models.add_issue_request import AddIssueRequest
from openapi_server.models.update_issue_request import UpdateIssueRequest

logging.basicConfig(level=logging.INFO)

def get_connection():
    """
    @brief Stellt eine Verbindung zur Neon-Datenbank her.
    """
    logging.info("Stelle Verbindung zur Datenbank her.")
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def add_issue(body):
    """
    @brief Erstellt ein neues Issue.

    @param body: JSON mit description und column_id
    @return: JSON mit Erfolgsmeldung und issue_id
    """
    logging.info("Empfange Anfrage zum Erstellen eines Issues.")
    description = body.get("description")
    column_id = body.get("column_id")

    if not description or column_id is None:
        logging.warning("description oder column_id fehlt.")
        abort(400, "description und column_id sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info("Füge neues Issue in die Datenbank ein.")

        cur.execute("""
            INSERT INTO issues (description, cid)
            VALUES (%s, %s)
            RETURNING iid
        """, (description, column_id))
        issue_id = cur.fetchone()[0]

        conn.commit()
        logging.info("Issue wurde erfolgreich erstellt.")
        return jsonify({"success": True, "issue_id": issue_id}), 201

    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Erstellen des Issues: {str(e)}")
        abort(500, f"Issue konnte nicht erstellt werden: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")

def update_issue(body):
    """
    @brief Aktualisiert die Beschreibung eines Issues.

    @param body: JSON mit issue_id und description
    @return: JSON mit Erfolgsmeldung
    """
    logging.info("Empfange Anfrage zum Aktualisieren eines Issues.")
    issue_id = body.get("id")
    new_description = body.get("description")

    if not issue_id or new_description is None:
        logging.warning("issue_id oder description fehlt.")
        abort(400, "issue_id und description sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info(f"Aktualisiere Issue mit ID {issue_id}.")

        cur.execute("""
            UPDATE issues
            SET description = %s
            WHERE iid = %s
        """, (new_description, issue_id))

        conn.commit()
        logging.info("Issue wurde erfolgreich aktualisiert.")
        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Aktualisieren des Issues: {str(e)}")
        abort(500, f"Issue konnte nicht aktualisiert werden: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")

def delete_issue(id_):
    """
    @brief Löscht ein Issue.

    @param id_: iid
    @return: JSON mit Erfolgsmeldung
    """
    logging.info(f"Empfange Anfrage zum Löschen des Issues mit ID {id_}.")
    conn = get_connection()

    try:
        cur = conn.cursor()
        logging.info("Lösche Issue aus der Datenbank.")

        cur.execute("DELETE FROM issues WHERE iid = %s", (id_,))
        if cur.rowcount == 0:
            logging.warning("Issue wurde nicht gefunden.")
            abort(404, "Issue wurde nicht gefunden.")
            
        conn.commit()
        logging.info("Issue wurde erfolgreich gelöscht.")
        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        logging.error(f"Fehler beim Löschen des Issues: {str(e)}")
        abort(500, f"Issue konnte nicht gelöscht werden: {str(e)}")

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")