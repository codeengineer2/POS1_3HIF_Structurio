from openapi_server.models.base_model import Model

class RegisterRequest(Model):
    def __init__(self, surname=None, lastname=None, email=None, password=None):
        self.openapi_types = {
            'surname': str,
            'lastname': str,
            'email': str,
            'password': str
        }
        self.attribute_map = {
            'surname': 'surname',
            'lastname': 'lastname',
            'email': 'email',
            'password': 'password'
        }

        self.surname = surname
        self.lastname = lastname
        self.email = email
        self.password = password