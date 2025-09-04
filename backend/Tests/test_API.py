import socket
import time
import json

IP = "127.0.0.1"
PORT = 3391

DISCOVERY_PORT = 50130
BUFFER_SIZE = 1024
DISCOVERY_MESSAGE = "DISCOVER_LOCAL_HOMESERVER"

# approve testcase if API worked but context not found
DEFAULT_ALLOW_NOT_FOUND = True

class bcolors:  
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

totalRunTime = 0.0

def test_StartProcess():
    ans = ConnectToAPI('{"Path":"/api/process/start", "Type":"UPDATE", "Data":"Minecraft Server"}')
    assert isAnswerSuccess(ans)
    

def test_GetProcesses():
    ans = ConnectToAPI('{"Path":"/api/processes", "Type":"GET", "Data":"Test"}')
    assert isAnswerSuccess(ans)
    

def test_GetProcessStatus():
    ans = ConnectToAPI('{"Path":"/api/processes/status", "Type":"GET", "Data":"Test"}')
    assert isAnswerSuccess(ans)


def test_LastLogs():
    ans = ConnectToAPI('{"Path":"/api/process/lastlogs", "Type":"GET", "Data":"Minecraft Server"}')
    assert isAnswerSuccess(ans)
    
    
def test_LastErrors():
    ans = ConnectToAPI('{"Path":"/api/process/lasterrors", "Type":"GET", "Data":"Minecraft Server"}')
    assert isAnswerSuccess(ans)
    
def test_InvalidProcesses():
    ans = ConnectToAPI('{"Path":"/api/processes", "Type":"POST", "Data":"Invalid Data"}')
    assert not isAnswerSuccess(ans, False)
    
def test_InvalidPath():
    ans = ConnectToAPI('{"Path":"/api/fsdfasdf", "Type":"GET", "Data":"Invalid Data"}')
    assert not isAnswerSuccess(ans, False)
    
def test_InvalidType():
    ans = ConnectToAPI('{"Path":"/api/processes", "Type":"migga", "Data":"Invalid Data"}')
    assert not isAnswerSuccess(ans, False)
    
def test_StopProcess():
    ans = ConnectToAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Test"}')
    assert isAnswerSuccess(ans)
    
def test_ProcessInput():
    ans = ConnectToAPI('{"Path":"api/process/input", "Type":"POST", "Data":"{\\"ProcessTag\\": \\"Minecraft Server\\", \\"Input\\": \\"say hello API\\"}"}')
    assert isAnswerSuccess(ans)
    
def test_ProcessStop():
    ans = ConnectToAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Minecraft Server"}')
    assert isAnswerSuccess(ans)
    
def isAnswerSuccess(ans: json, Allow_not_found: bool = DEFAULT_ALLOW_NOT_FOUND) -> bool:
    '''
    Getting the API answer and returning if the answer is status code is success
    <br>ans: Server Answer
    <br>Allow_not_found: Allowing the answer to be 404 if the API path found but context wasn't
    '''
    # if the answer isn't unknown so the API worked but the data was not found
    if (Allow_not_found and ans["StatusCode"] == 404):
        if "unknown" not in ans["Data"]:
            return True
        
    return ans["StatusCode"] < 300


'''
def main():
    global totalRunTime
    
    print("Searching for server")
    
    print("=====================Starting API Tests=======================")
    ConnectToAPI('{"Path":"/api/process/start", "Type":"UPDATE", "Data":"Minecraft Server"}')
    ConnectToAPI('{"Path":"/api/processes", "Type":"GET", "Data":"Test"}')
    ConnectToAPI('{"Path":"/api/processes/status", "Type":"GET", "Data":"Test"}')
    ConnectToAPI('{"Path":"/api/process/lastlogs", "Type":"GET", "Data":"Minecraft Server"}')
    ConnectToAPI('{"Path":"/api/process/lasterrors", "Type":"GET", "Data":"Minecraft Server"}')
    ConnectToAPI('{"Path":"/api/fsdfasdf", "Type":"GET", "Data":"Invalid Data"}')
    ConnectToAPI('{"Path":"/api/processes", "Type":"migga", "Data":"Invalid Data"}')
    ConnectToAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Test"}')
    time.sleep(5)
    ConnectToAPI('{"Path":"api/process/input", "Type":"POST", "Data":"{\\"ProcessTag\\": \\"Minecraft Server\\", \\"Input\\": \\"say hello API\\"}"}')
    time.sleep(10)
    ConnectToAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Minecraft Server"}')
    
    print(f"Execution time: {totalRunTime:.4f} seconds")
'''

def ConnectToAPI(message):
    global totalRunTime
    
    print ("Testing API with message:", message)
    try:
        print("Trying to connect: " + str((IP, PORT)) )
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(2)
            s.connect((IP, PORT))
            s.sendall(message.encode())
            
            StartTime = time.perf_counter()
            data = s.recv(1024)
            elapsed_time = time.perf_counter() - StartTime
            totalRunTime += elapsed_time
            print(f"{bcolors.OKGREEN} {data.decode()} {bcolors.ENDC}")
            print(f"{bcolors.OKCYAN}{bcolors.BOLD} Answer time: {(elapsed_time * 1000):.0f} ms{bcolors.ENDC}")

    except Exception as err:
        raise Exception (f"Failed to message API: {err}") 

    print("\n\n")
    return json.loads(data.decode())


if __name__ == "__main__":
    print("Pls use pytest")
    exit(-1)