import psycopg2
import random
from datetime import date, time, timedelta

conn_str = (
    "postgresql://structure_owner:npg_cEPXthQ49IRm@"
    "ep-calm-grass-a272ihxj-pooler.eu-central-1.aws.neon.tech/"
    "structure?sslmode=require"
)

try:
    conn = psycopg2.connect(conn_str)
    cur = conn.cursor()

    cur.execute("""
        CREATE TABLE IF NOT EXISTS users (
            uid SERIAL PRIMARY KEY,
            firstname VARCHAR(100) NOT NULL,
            lastname VARCHAR(100) NOT NULL,
            email VARCHAR(150) UNIQUE NOT NULL,
            password_hash TEXT NOT NULL,
            birthdate DATE NOT NULL
        );

        CREATE TABLE IF NOT EXISTS projects (
            pid SERIAL PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            description TEXT,
            color VARCHAR(20),
            owner_uid INT REFERENCES users(uid) ON DELETE SET NULL
        );

        CREATE TABLE IF NOT EXISTS boards (
            bid SERIAL PRIMARY KEY,
            pid INT UNIQUE NOT NULL REFERENCES projects(pid) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS columns (
            cid SERIAL PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            bid INT NOT NULL REFERENCES boards(bid) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS issues (
            iid SERIAL PRIMARY KEY,
            description TEXT NOT NULL,
            cid INT NOT NULL REFERENCES columns(cid) ON DELETE CASCADE
        );
    """)

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO users (firstname, lastname, email, password_hash, birthdate) VALUES (%s, %s, %s, %s, %s)
        """, (f"User{i}", f"Test{i}", f"user{i}@example.com", f"hashed_pw_{i}", date(2000+i, 1, 1)))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO projects (name, description, color, owner_uid) VALUES (%s, %s, %s, %s)
        """, (f"Project{i}", f"Description for project {i}", "#FF5733", i))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO boards (pid) VALUES (%s)
        """, (i,))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO columns (name, bid) VALUES (%s, %s)
        """, (f"Column{i}", i))

    for i in range(1, 11):
        cur.execute("""
            INSERT INTO issues (description, cid) VALUES (%s, %s)
        """, (f"Issue description {i}", i))

    conn.commit()
    cur.close()
    conn.close()
    print("Tabellen und Testdaten erfolgreich erstellt.")

except Exception as e:
    print("Fehler beim Ausführen des Skripts:")
    print(e)