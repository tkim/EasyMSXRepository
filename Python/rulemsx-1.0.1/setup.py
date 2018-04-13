# rulemsx setup.py
from distutils.core import setup
setup(
    name = "rulemsx",
    packages = ["rulemsx"],
    version = "1.0.0",
    description = "RuleMSX rule engine",
    author = "Richard Clegg",
    author_email = "rclegg2@bloomberg.net",
    url = "https://github.com/rikclegg/py_RuleMSX",
    keywords = ["Bloomberg API", "blpapi", "rule engine"],
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
RuleMSX rule engine
-------------------

RuleMSX is designed for integration with the Bloomberg API using EasyMSX 
and EasyMSX to build applications around a set of user defined rules,
conditions and actions.

This version requires Python 3 or later
"""
)
