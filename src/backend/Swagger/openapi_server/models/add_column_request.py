from openapi_server.models.base_model import Model

class AddColumnRequest(Model):
    def __init__(self, board_id=None, name=None):
        self.openapi_types = {'board_id': int, 'name': str}
        self.attribute_map = {'board_id': 'board_id', 'name': 'name'}

        self.board_id = board_id
        self.name = name