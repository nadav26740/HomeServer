import socket
import time

IP = "127.0.0.1"
PORT = 3391

BROADCAST_ADDRESS = ('255.255.255.255', 50130)
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


def main():
    if discover_server():
        print(f"{bcolors.OKGREEN} Server found {IP} {bcolors.ENDC}") 

    else:
        print(f"{bcolors.FAIL} Failed to find server! {bcolors.ENDC}")
        exit(1)


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
    broadcast_address = BROADCAST_ADDRESS

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

        IP = str(server_addr[0])
        
        Succeeded = True
        
    except socket.timeout:
        print("No server response received within timeout.")
    finally:
        client_socket.close()
    return Succeeded


if __name__ == "__main__":
    main()