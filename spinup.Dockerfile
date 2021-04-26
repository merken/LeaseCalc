FROM python:3.9-slim-buster

RUN pip3 install azure-storage-blob
RUN pip3 install aiohttp
RUN pip3 install python-dotenv

COPY LeaseCalc.SpinUp/spinup.py .
COPY LeaseCalc.Functions/journey.v1.json .
COPY LeaseCalc.Functions/journey.v2.json .

ENTRYPOINT ["python3", "spinup.py"]