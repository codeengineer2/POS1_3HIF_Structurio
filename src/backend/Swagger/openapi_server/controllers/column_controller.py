import psycopg2
from flask import request, jsonify, abort
from openapi_server.models.add_column_request import AddColumnRequest
from openapi_server.models.update_column_request import UpdateColumnRequest

def get_connection():
    """
    @brief Stellt eine Verbindung zur Neon-Datenbank her.
    """
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
    board_id = body.get("board_id")
    name = body.get("name")

    if not board_id or not name:
        abort(400, "board_id und name sind erforderlich.")

    conn = get_connection()
    
    try:
        cur = conn.cursor()

        cur.execute("INSERT INTO columns (bid, name) VALUES (%s, %s) RETURNING cid", (board_id, name))
        cid = cur.fetchone()[0]

        conn.commit()

        return jsonify({"cid": cid, "name": name}), 201
    
    except Exception as e:
        conn.rollback()
        abort(500, f"Fehler: {str(e)}")

    finally:
        cur.close()
        conn.close()

def update_column(body):
    """
    @brief Aktualisiert den Namen einer Spalte.

    @param body: JSON mit cid und name
    @return: JSON mit Erfolgsmeldung
    """
    cid = body.get("id")
    name = body.get("name")

    if not cid or not name:
        abort(400, "id und name sind erforderlich.")

    conn = get_connection()

    try:
        cur = conn.cursor()
        
        cur.execute("UPDATE columns SET name = %s WHERE cid = %s", (name, cid))
        if cur.rowcount == 0:
            abort(404, "Spalte nicht gefunden.")

        conn.commit()

        return jsonify({"success": True}), 200
    
    except Exception as e:
        conn.rollback()
        abort(500, f"Fehler: {str(e)}")

    finally:
        cur.close()
        conn.close()