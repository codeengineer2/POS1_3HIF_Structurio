import psycopg2
import logging
import hashlib
from openapi_server.models.email_request import EmailRequest
from openapi_server.models.login_request import LoginRequest
from openapi_server.models.register_request import RegisterRequest

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

def hash_password(password):
    """
    @brief Hasht ein Passwort mit SHA-256.

    @param password: Das zu hashende Passwort im Klartext.
    @type password: str
    @return: Der SHA-256 Hash des Passworts als hexadezimale Zeichenkette.
    @rtype: str
    """
    logging.info("Hashing Passwort...")
    return hashlib.sha256(password.encode("utf-8")).hexdigest()

def auth_check_email_post(body):
    """
    @brief Prüft ob eine bestimmte EMail existiert.

    @param body: EmailRequest-Objekt oder dict mit email
    @return: JSON mit Erfolgsmeldung
    """
    logging.info("Prüfe ob E-Mail vorhanden ist.")
    email = body.get("email") if isinstance(body, dict) else body.email

    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT 1 FROM users WHERE email = %s", (email,))
    found = cur.fetchone()
    cur.close()
    conn.close()

    if found:
        logging.info("E-Mail wurde gefunden.")
        return {"exists": True}, 200
    else:
        logging.warning("E-Mail wurde nicht gefunden.")
        return {"error": "E-Mail nicht gefunden"}, 404

def auth_login_post(body):
    """
    @brief Führt einen Anmeldevorgang mit EMail und Passwort durch.
    Gibt bei Erfolg vollständige Benutzerdaten und Projektdaten zurück sonst Fehlermeldung.

    @param body: LoginRequest-Objekt oder dict mit email und password
    @return: JSON mit Benutzer und Projekten oder Fehlermeldung
    """
    logging.info("Verarbeite Login-Anfrage.")
    email = body.get("email") if isinstance(body, dict) else body.email
    password = body.get("password") if isinstance(body, dict) else body.password
    password = hash_password(password)

    conn = get_connection()
    cur = conn.cursor()

    cur.execute("""
        SELECT uid, firstname, lastname, email, birthdate
        FROM users
        WHERE email = %s AND password_hash = %s
    """, (email, password))
    user_row = cur.fetchone()

    if not user_row:
        cur.close()
        conn.close()
        logging.warning("Login fehlgeschlagen.")
        return {
            "success": False,
            "code": "LOGIN_FAILED",
            "message": "E-Mail oder Passwort ungültig"
        }, 401

    uid, firstname, lastname, email, birthdate = user_row
    logging.info(f"Login erfolgreich für Benutzer-ID {uid}.")

    cur.execute("""
        SELECT pid, name, description, color FROM projects
        WHERE owner_uid = %s
    """, (uid,))
    
    projects = []
    
    for pid, name, description, color in cur.fetchall():
        cur.execute("SELECT bid FROM boards WHERE pid = %s", (pid,))
        board_row = cur.fetchone()
        board_id = board_row[0] if board_row else None
        board_obj = {"id": board_id, "project_id": pid, "columns": []}

        cur.execute("SELECT cid, name FROM columns WHERE bid = %s", (board_id,))
        
        for cid, cname in cur.fetchall():
            cur.execute("SELECT iid, description FROM issues WHERE cid = %s", (cid,))
            issues = [{"id": iid, "description": desc, "column_id": cid} for iid, desc in cur.fetchall()]
            board_obj["columns"].append({
                "id": cid, "name": cname, "board_id": board_id, "issues": issues
            })

        projects.append({
            "pid": pid,
            "name": name,
            "description": description,
            "color": color,
            "owner_uid": uid,
            "board": board_obj
        })

    cur.close()
    conn.close()
    logging.info("Benutzerdaten und Projekte erfolgreich geladen.")

    return {
        "success": True,
        "user": {
            "uid": uid,
            "firstname": firstname,
            "lastname": lastname,
            "email": email,
            "birthdate": str(birthdate)
        },
        "projects": projects
    }, 200

def auth_register_post(body):
    """
    @brief Erstellt einen Benutzer mit den übergebenen Daten.

    @param body: RegisterRequest-Objekt oder dict mit Vorname, Nachname, Email, Passwort, Geburtsdatum
    @return: JSON mit Benutzer oder Fehlermeldung
    """
    logging.info("Verarbeite Registrierungsanfrage.")
    firstname = body.get("firstname") if isinstance(body, dict) else body.firstname
    lastname = body.get("lastname") if isinstance(body, dict) else body.lastname
    email = body.get("email") if isinstance(body, dict) else body.email
    password = body.get("password") if isinstance(body, dict) else body.password
    birthdate = body.get("birthdate") if isinstance(body, dict) else body.birthdate

    password = hash_password(password)

    conn = get_connection()
    cur = conn.cursor()
    
    try:
        cur.execute(
            """
            INSERT INTO users (firstname, lastname, email, password_hash, birthdate)
            VALUES (%s, %s, %s, %s, %s)
            RETURNING uid
            """,
            (firstname, lastname, email, password, birthdate)
        )
        uid = cur.fetchone()[0]
        conn.commit()
        logging.info(f"Benutzer erfolgreich registriert mit ID {uid}.")

        return {
            "success": True,
            "user": {
                "uid": uid,
                "firstname": firstname,
                "lastname": lastname,
                "email": email,
                "birthdate": str(birthdate)
            }
        }, 201

    except psycopg2.errors.UniqueViolation:
        conn.rollback()
        logging.warning("Registrierung fehlgeschlagen: E-Mail existiert bereits.")
        return {
            "success": False,
            "code": "EMAIL_EXISTS",
            "message": "Diese E-Mail ist bereits registriert"
        }, 409

    finally:
        cur.close()
        conn.close()
        logging.info("Verbindung geschlossen.")