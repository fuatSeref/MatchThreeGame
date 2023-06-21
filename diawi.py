#!/usr/bin/env python

import os
import sys
import time
import argparse
import requests
import json
import string
import random
import re

from bs4 import BeautifulSoup

TOKEN_URL = "https://www.diawi.com/"
UPLOAD_URL = "https://upload.diawi.com/plupload.php"
POST_URL = "https://upload.diawi.com/plupload"
STATUS_URL = "https://upload.diawi.com/status"

DIRECTORY= "./Builds/Development/"

set_debug = False

def debug(message):
    if set_debug is True:
        print("DEBUG : {}".format(message))

def check_len(list):
    if len(list) == 0:
        debug("Folder is empty")
        sys.exit(1)

def validate_file():
    global file_name
    global file_directory

    files = os.listdir(DIRECTORY)

    check_len(files);

    for index in range(len(files)):
        name, extention = os.path.splitext(files[index])
        if validate_extension(extention):
            debug("found file {}".format(files[index]))
            file_index = index
            is_found = True

    if is_found is False:
        debug("Folder doesn't contain any .ipa files")
        sys.exit(1)

    file_directory = DIRECTORY + files[file_index]
    file_name = files[file_index]

def validate_extension(extension):
    if extension == ".ipa" or extension == ".apk":
        debug("valid file extention : {}".format(extension))
        return True
    else:
        return False

def create_tmp_file_name(args):
    name, extention = os.path.splitext(args)

    tmp_file_name = "o_{}{}".format(''.join(random.SystemRandom().choice(
        string.ascii_lowercase + string.digits) for _ in range(29)), extention)

    debug("temp file name : {}".format(tmp_file_name))

    return tmp_file_name

def file_upload(tmp_file_name, token):
    files = {"file": open(file_directory, "rb")}
    upload_data = {"name": tmp_file_name}
    upload_params = {"token": token}

    debug("Uploading File...")
    r = requests.post(UPLOAD_URL, params=upload_params, files=files, data=upload_data)

    debug("file upload responce code : {}".format(r.status_code))
    debug("file upload responce text : {}".format(r.text))

    if r.status_code != 200:
        debug("Failed to upload file!")
        sys.exit(1)

def file_post(tmp_file_name, token):
    post_data = {
        'token': token,
        'uploader_0_tmpname': tmp_file_name,
        'uploader_0_name': file_name,
        'uploader_0_status': 'done',
        'uploader_count': '1',
        'comment': '',
        'email': '',
        'password': '',
        'notifs': 'off',
        'udid': 'off',
        'wall': 'off'
    }

    debug("Posting File...")
    r = requests.post(POST_URL, data=post_data)

    debug("file post responce code : {}".format(r.status_code))
    debug("file post responce text : {}".format(r.text))

    if r.status_code != 200:
        debug("Failed to post file!")
        sys.exit(1)

    json_result = json.loads(r.text)
    debug(json_result["job"])
    return json_result["job"]

def get_job_status(token_id, job_id):
    debug("Getting status...")

    status_params = {'token': token_id, 'job': job_id}
    while True:

        r = requests.get(STATUS_URL, params=status_params)

        debug("job status responce code : {}".format(r.status_code))
        debug("job status responce text : {}".format(r.text))

        if r.status_code == 200:
            json_result = json.loads(r.text)
            if json_result["status"] == 2000:
                print("{}".format(json_result["hash"]))
                break
            else:
                debug("App is not ready, waiting before retry.")
                time.sleep(5)
        else:
            debug("Server encounted an error")
            sys.exit(1)

def main():
    validate_file()
    tmp_file_name = create_tmp_file_name(file_directory)
    token = "FXgS7u4uqTXdQebKpIkpxiRe3nPNZdGzUJmljHYDRh"
    file_upload(tmp_file_name, token)
    job_id = file_post(tmp_file_name, token)
    get_job_status(token, job_id)

if __name__ == '__main__':
    main()
