import psycopg2
from flask import request, jsonify, abort
from openapi_server.models.project_request import ProjectRequest

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

        cur.execute("INSERT INTO boards (pid) VALUES (%s)", (pid,))

        conn.commit()

        return jsonify({"success": True, "pid": pid}), 201

    except Exception as e:
        conn.rollback()
        abort(500, f"Fehler beim Anlegen des Projekts: {str(e)}")

    finally:
        cur.close()
        conn.close()