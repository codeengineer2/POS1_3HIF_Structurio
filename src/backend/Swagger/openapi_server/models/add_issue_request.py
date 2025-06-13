from openapi_server.models.base_model import Model

class AddIssueRequest:
    def __init__(self, column_id: int, description: str):
        self.column_id = column_id
        self.description = description