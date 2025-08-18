import socket
import time
import json

IP = "127.0.0.1"
PORT = 3391

DISCOVERY_PORT = 50130
BUFFER_SIZE = 1024
DISCOVERY_MESSAGE = "DISCOVER_LOCAL_HOMESERVER"


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

def main():
    global totalRunTime
    
    print("Searching for server")
    if discover_server():
        print(f"{bcolors.OKGREEN} Server found {IP}:{str(PORT)} {bcolors.ENDC}") 

    else:
        print(f"{bcolors.FAIL} Failed to find server! {bcolors.ENDC}")
        return -1
    
    print("=====================Starting API Tests=======================")
    TestAPI('{"Path":"/api/process/start", "Type":"UPDATE", "Data":"Minecraft Server"}')
    TestAPI('{"Path":"/api/processes", "Type":"GET", "Data":"Test"}')
    TestAPI('{"Path":"/api/processes/status", "Type":"GET", "Data":"Test"}')
    TestAPI('{"Path":"/api/process/lastlogs", "Type":"GET", "Data":"Minecraft Server"}')
    TestAPI('{"Path":"/api/process/lasterrors", "Type":"GET", "Data":"Minecraft Server"}')
    TestAPI('{"Path":"/api/processes", "Type":"POST", "Data":"Invalid Data"}')
    TestAPI('{"Path":"/api/fsdfasdf", "Type":"GET", "Data":"Invalid Data"}')
    TestAPI('{"Path":"/api/processes", "Type":"migga", "Data":"Invalid Data"}')
    TestAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Test"}')
    time.sleep(5)
    TestAPI('{"Path":"api/process/input", "Type":"POST", "Data":"{\\"ProcessTag\\": \\"Minecraft Server\\", \\"Input\\": \\"say hello API\\"}"}')
    time.sleep(10)
    TestAPI('{"Path":"/api/process/stop", "Type":"UPDATE", "Data":"Minecraft Server"}')
    
    print(f"Execution time: {totalRunTime:.4f} seconds")


def TestAPI(message):
    global totalRunTime
    
    print ("Testing API with message:", message)
    try:
        print("Trying to connect: " + str((IP, PORT)) )
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.connect((IP, PORT))
            s.sendall(message.encode())
            
            StartTime = time.perf_counter()
            data = s.recv(1024)
            elapsed_time = time.perf_counter() - StartTime
            totalRunTime += elapsed_time
            print(f"{bcolors.OKGREEN} {data.decode()} {bcolors.ENDC}")
            print(f"{bcolors.OKCYAN}{bcolors.BOLD} Answer time: {(elapsed_time * 1000):.0f} ms{bcolors.ENDC}")

    except Exception as err:
        print(f"{bcolors.FAIL} Failed to message API: {err} {bcolors.ENDC}") 

    print("\n\n")

def discover_server(timeout=5):
    global IP
    global PORT
    global totalRunTime
    
    # Create a UDP socket
    Succeeded = False
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    client_socket.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
    client_socket.settimeout(timeout)  # timeout in seconds

    # Broadcast address
    broadcast_address = ('192.168.50.255', DISCOVERY_PORT)

    try:
        # Send discovery message
        client_socket.sendto(DISCOVERY_MESSAGE.encode('utf-8'), broadcast_address)
        print(f"Broadcasted discovery request to {broadcast_address}")

        # Wait for response from server
        data, server_addr = client_socket.recvfrom(BUFFER_SIZE)

        StartTime = time.perf_counter()
        response = data.decode('utf-8')
        elapsed_time = time.perf_counter() - StartTime
        totalRunTime += elapsed_time

        print(f"Received response: '{response}' from {server_addr[0]}:{server_addr[1]}")
        print(f"{bcolors.OKCYAN}{bcolors.BOLD} Answer time: {(elapsed_time * 1000):.0f} ms{bcolors.ENDC}")

        PORT = json.loads(response)["Port"]
        IP = str(server_addr[0])
        
        Succeeded = True
        
    except socket.timeout:
        print("No server response received within timeout.")
    finally:
        client_socket.close()
    return Succeeded


if __name__ == "__main__":
    main()