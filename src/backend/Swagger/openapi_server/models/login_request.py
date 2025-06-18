from openapi_server.models.base_model import Model

class LoginRequest(Model):
    """
    @brief Ist eine Anfrage mit EMail und Passwort.

    @param email: Die EMail des Benutzers.
    @type email: str

    @param password: Das Passwort des Benutzers.
    @type password: str
    """
    def __init__(self, email=None, password=None):
        self.openapi_types = {
            'email': str,
            'password': str
        }
        self.attribute_map = {
            'email': 'email',
            'password': 'password'
        }

        self.email = email
        self.password = password