import os
import aiohttp
import asyncio
import traceback

class Spinup(object):
    connection_string: str

    def __init__(self, connection_string):
        self.connection_string = connection_string

    async def ensure_container(self, container_name):
        from azure.storage.blob.aio import BlobServiceClient
        blob_service_client = BlobServiceClient.from_connection_string(
            self.connection_string)

        async with blob_service_client:
            blob_container_client = blob_service_client.get_container_client(
                container_name)
            exists = await blob_container_client.exists()
            if not exists:
                await blob_container_client.create_container()
            return blob_container_client

    async def upload_to_container(self, container_name: str, file: str):
        from azure.storage.blob.aio import BlobServiceClient, BlobClient

        blob_service_client = BlobServiceClient.from_connection_string(
            self.connection_string)

        blob_client = blob_service_client.get_blob_client(
            container=container_name, blob=file)

        async with blob_client:
            exists = await blob_client.exists()
            if exists:
                return

            with open(file, "rb") as data:
                await blob_client.upload_blob(data)

# main entry point, could be named other than main
async def main():
    print('Starting spinup.py:')
    try:
        from azure.storage.blob.aio import BlobServiceClient

        connection_string = os.getenv('AzureStorageConnection')
        if connection_string is None :
            raise ValueError("AzureStorageConnection environment variable was not provided")

        container_name = "journeys"
        journeys_to_upload = ["journey.v1.json", "journey.v2.json"]
        spinup = Spinup(connection_string)

        print('Ensuring :' + container_name)
        await spinup.ensure_container(container_name)
        print('Ensuring :' + container_name)
        for upload in journeys_to_upload:
            print('Uploading :' + container_name)
            await spinup.upload_to_container(container_name, upload)

    except Exception as ex:
        traceback.print_exc()

# Run the main entrypoint asynchonously
if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main())
