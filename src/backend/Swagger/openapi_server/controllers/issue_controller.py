import psycopg2
from flask import request, jsonify, abort
from openapi_server.models.add_issue_request import AddIssueRequest
from openapi_server.models.update_issue_request import UpdateIssueRequest

def get_connection():
    """
    Stellt eine Verbindung zur Neon-Datenbank her.
    """
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def add_issue(body):
    """
    Erstellt ein neues Issue.

    :param body: JSON mit description und column_id
    :return: JSON mit Erfolgsmeldung und issue_id
    """
    description = body.get("description")
    column_id = body.get("column_id")

    if not description or column_id is None:
        abort(400, "description und column_id sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("""
            INSERT INTO issues (description, cid)
            VALUES (%s, %s)
            RETURNING iid
        """, (description, column_id))
        issue_id = cur.fetchone()[0]

        conn.commit()

        return jsonify({"success": True, "issue_id": issue_id}), 201

    except Exception as e:
        conn.rollback()
        abort(500, f"Issue konnte nicht erstellt werden: {str(e)}")

    finally:
        cur.close()
        conn.close()

def update_issue(body):
    """
    Aktualisiert die Beschreibung eines Issues.

    :param body: JSON mit issue_id und description
    :return: JSON mit Erfolgsmeldung
    """
    issue_id = body.get("id")
    new_description = body.get("description")

    if not issue_id or new_description is None:
        abort(400, "issue_id und description sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("""
            UPDATE issues
            SET description = %s
            WHERE iid = %s
        """, (new_description, issue_id))

        conn.commit()

        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        abort(500, f"Issue konnte nicht aktualisiert werden: {str(e)}")

    finally:
        cur.close()
        conn.close()

def delete_issue(id_):
    """
    Löscht ein Issue.

    :param id_: iid
    :return: JSON mit Erfolgsmeldung
    """
    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("DELETE FROM issues WHERE iid = %s", (id_,))
        if cur.rowcount == 0:
            abort(404, "Issue wurde nicht gefunden.")
            
        conn.commit()

        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        abort(500, f"Issue konnte nicht gelöscht werden: {str(e)}")

    finally:
        cur.close()
        conn.close()