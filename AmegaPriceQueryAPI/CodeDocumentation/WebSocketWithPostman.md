# Connecting to the SignalR WebSocket using Postman

This guide will walk you through the process of connecting to the WebSocket using Postman and performing subscribe and unsubscribe actions.

## Step 1: Open Postman and Set Up WebSocket Request

1. **Open Postman.**
2. **Create a new request:**
    - Click on `New` > `WebSocket Request`.
3. **Enter the WebSocket URL:**
    - Set the WebSocket URL to: `wss://localhost:7181/signalrhub`.

## Step 2: Connect to the WebSocket

1. **Connect to the WebSocket:**
    - Click the `Connect` button in Postman.
2. **Send the initial handshake message:**
    - In the message input field, enter the following message:
      ```json
      {"protocol":"json","version":1}
      ```
    - Ensure the `ASCII character 0x1E` (represented by ``) is included at the end of the message.
    - Click `Send`.

## Step 3: Subscribe to an Instrument

1. **Subscribe to an instrument:**
    - In the message input field, enter the following message:
      ```json
      {
        "type": 1,
        "target": "Subscribe",
        "arguments": ["btcusdt"]
      }
      ```
    - Ensure the `ASCII character 0x1E` (represented by ``) is included at the end of the message.
    - Click `Send`.

## Step 4: Unsubscribe from an Instrument

1. **Unsubscribe from an instrument:**
    - In the message input field, enter the following message:
      ```json
      {
        "type": 1,
        "target": "UnSubscribe",
        "arguments": ["btcusdt"]
      }
      ```
    - Ensure the `ASCII character 0x1E` (represented by ``) is included at the end of the message.
    - Click `Send`.

## Additional Notes

- The `ASCII character 0x1E` (represented by ``) must be included at the end of all messages.
- Replace `"btcusdt"` with the desired instrument symbol as needed.

By following these steps, you can successfully connect to the WebSocket using Postman and manage subscriptions to different instruments.

For a clearer description, view this documentation from [TrailHead](https://trailheadtechnology.com/using-postman-with-signalr-websockets-development/)
