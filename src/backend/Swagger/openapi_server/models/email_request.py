from openapi_server.models.base_model import Model

class EmailRequest(Model):
    """
    Ist eine Anfrage die eine EMail hat.

    :param email: Die EMail die überprüft wird.
    :type email: str
    """
    def __init__(self, email=None):
        self.openapi_types = {
            'email': str
        }
        self.attribute_map = {
            'email': 'email'
        }
        self.email = email