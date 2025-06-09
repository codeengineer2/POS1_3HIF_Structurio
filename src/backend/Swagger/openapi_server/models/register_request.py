from openapi_server.models.base_model import Model

class RegisterRequest(Model):
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