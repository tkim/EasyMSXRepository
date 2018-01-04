# easymkt setup.py
from distutils.core import setup
setup(
    name = "easymkt",
    packages = ["easymkt"],
    version = "1.0.0",
    description = "EasyMKT caching model",
    author = "Richard Clegg",
    author_email = "rclegg2@bloomberg.net",
    url = "https://github.com/rikclegg/py_EasyMKT",
    keywords = ["Bloomberg API", "blpapi", "mktdata", "market data"],
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
EasyMKT
-------

EasyMKT is designed for integration with the Bloomberg API and 
the Market Data service. It provides a local cache of for market
data field values.

This version requires Python 3 or later
"""
)
