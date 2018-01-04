# easymsx setup.py
from distutils.core import setup
setup(
    name = "easymsx",
    packages = ["easymsx"],
    version = "1.0.0",
    description = "EasyMSX caching model",
    author = "Richard Clegg",
    author_email = "rclegg2@bloomberg.net",
    url = "https://github.com/rikclegg/py_EasyMSX",
    keywords = ["Bloomberg API", "blpapi", "EMSX", "EMSX API", "EMSXAPI"],
    classifiers = [
        "Programming Language :: Python",
        "Programming Language :: Python :: 3",
        "Development Status :: Alpha",
        "Environment :: Other Environment",
        "Intended Audience :: Developers",
        "Operating System :: OS Independent",
        "Topic :: Software Development :: Libraries :: Python Modules",
        ],
    long_description = """\
EasyMSX
-------

EasyMSX is designed for integration with the Bloomberg API and 
the EMSX API service. It provides a complete local cache of EMSX
order and route data.

This version requires Python 3 or later
"""
)
