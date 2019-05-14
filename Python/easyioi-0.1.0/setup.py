# easyioi setup.py
from distutils.core import setup
setup(
    name = "easyioi",
    packages = ["easyioi"],
    version = "0.1.0",
    description = "EasyIOI caching model",
    author = "Terrence C. Kim",
    author_email = "tkim94@bloomberg.net",
    url = "https://github.com/tkim/EasyMSXRepository/tree/master/Python/easyioi-0.1.0",
    keywords = ["Bloomberg API", "blpapi", "ioi"],
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
EasyIOI
-------

EasyIOI is designed for integration with the Bloomberg API and 
the IOI data service. It provides a local cache of IOI data field
values.

This version requires Python 3 or later
"""
)
