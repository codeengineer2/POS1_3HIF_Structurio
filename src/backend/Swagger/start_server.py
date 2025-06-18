import openapi_server.__main__ as main
import logging
from logging.config import dictConfig

dictConfig({
    "version": 1,
    "formatters": {
        "default": {
            "format": "%(asctime)s %(levelname)-8s [%(name)s] %(message)s",
        },
    },
    "handlers": {
        "console": {
            "class": "logging.StreamHandler",
            "formatter": "default",
            "level": "DEBUG",
        },
        "file": {
            "class": "logging.handlers.RotatingFileHandler",
            "formatter": "default",
            "filename": "app.log",
            "maxBytes": 10*1024*1024,
            "backupCount": 5,
            "level": "INFO",
        },
    },
    "root": {
        "handlers": ["console", "file"],
        "level": "DEBUG",
    },
})

if __name__ == "__main__":
    main.main()

# $env:PYTHONPATH="."; python -m unittest discover openapi_server/test
