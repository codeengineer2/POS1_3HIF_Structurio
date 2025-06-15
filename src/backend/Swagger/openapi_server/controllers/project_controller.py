import psycopg2
from flask import request, jsonify, abort
from openapi_server.models.project_request import ProjectRequest
from openapi_server.models.update_project_request import UpdateProjectRequest

def get_connection():
    # egal weil egal
    conn_str = (
        "postgresql://structure_owner:npg_cEPXthQ49IRm@"
        "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
        "structure?sslmode=require"
    )
    return psycopg2.connect(conn_str)

def create_project(body):
    name = body.get("name")
    description = body.get("description")
    color = body.get("color")
    owner_uid = body.get("owner_uid")

    if name is None or color is None or owner_uid is None:
        abort(400, "Felder name, color und owner_uid sind erforderlich")

    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("""
            INSERT INTO projects (name, description, color, owner_uid)
            VALUES (%s, %s, %s, %s)
            RETURNING pid
        """, (name, description, color, owner_uid))
        pid = cur.fetchone()[0]

        cur.execute("INSERT INTO boards (pid) VALUES (%s) RETURNING bid", (pid,))
        bid = cur.fetchone()[0]

        conn.commit()

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
        abort(500, f"Fehler beim Anlegen des Projekts: {str(e)}")

    finally:
        cur.close()
        conn.close()

def update_project(body):
    pid = body.get("pid")
    name = body.get("name")
    description = body.get("description")
    color = body.get("color")

    if not pid or not name or not color:
        abort(400, "Felder pid, name und color sind erforderlich")

    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("SELECT pid FROM projects WHERE pid = %s", (pid,))
        if not cur.fetchone():
            abort(404, "Projekt nicht gefunden")

        cur.execute("""
            UPDATE projects
            SET name = %s, description = %s, color = %s
            WHERE pid = %s
        """, (name, description, color, pid))

        conn.commit()

        return jsonify({"success": True}), 200

    except Exception as e:
        conn.rollback()
        abort(500, f"Fehler beim Aktualisieren: {str(e)}")

    finally:
        cur.close()
        conn.close()

def delete_project(pid):
    conn = get_connection()

    try:
        cur = conn.cursor()

        cur.execute("SELECT pid FROM projects WHERE pid = %s", (pid,))
        if not cur.fetchone():
            abort(404, "Projekt nicht gefunden")

        cur.execute("DELETE FROM projects WHERE pid = %s", (pid,))

        conn.commit()

        return jsonify({"success": True}), 200

    except Exception as e:
        conn.rollback()
        abort(500, f"Fehler beim Löschen: {str(e)}")

    finally:
        cur.close()
        conn.close()