from openapi_server.models.base_model import Model

class RegisterRequest(Model):
    """
    @brief Ist eine Anfrage zur erstellung von einem neuen Benutzer.

    @param firstname: Der Vorname des Benutzers.
    @type firstname: str
    @param lastname: Der Nachname des Benutzers.
    @type lastname: str
    @param birthdate: Das Geburtsdatum des Benutzers.
    @type birthdate: str
    @param email: Die EMail des Benutzers.
    @type email: str
    @param password: Das Passwort des Benutzers.
    @type password: str
    """
    def __init__(self, firstname=None, lastname=None, email=None, password=None, birthdate=None):
        self.openapi_types = {
            'firstname': str,
            'lastname': str,
            'birthdate': str,
            'email': str,
            'password': str
        }
        self.attribute_map = {
            'firstname': 'firstname',
            'lastname': 'lastname',
            'birthdate': 'birthdate',
            'email': 'email',
            'password': 'password'
        }

        self.firstname = firstname
        self.lastname = lastname
        self.birthdate = birthdate
        self.email = email
        self.password = password