package server;

import javax.bluetooth.DiscoveryAgent;
import javax.bluetooth.LocalDevice;
import javax.bluetooth.UUID;

import javax.microedition.io.Connector;
import javax.microedition.io.StreamConnection;
import javax.microedition.io.StreamConnectionNotifier;

public class WaitThread implements Runnable {

    /** Constructor */
    public WaitThread() {
    	super();
    }

    @Override
    public void run() {
        waitForConnection();
    }

    /** Waiting for connection from devices */
    private void waitForConnection() {
        // retrieve the local Bluetooth device object
        LocalDevice local = null;

        StreamConnectionNotifier notifier;
        StreamConnection connection = null;

        // setup the server to listen for connection
        try {
            local = LocalDevice.getLocalDevice();
            local.setDiscoverable(DiscoveryAgent.GIAC);

            UUID uuid = new UUID("0000110100001000800000805F9B34FB", false);//UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");// "04c6093b-0000-1000-8000-00805f9b34fb"
            String url = "btspp://localhost:" + uuid.toString() + ";name=RemoteBluetooth";
            System.out.println("uuid is " + uuid.toString());
            System.out.println(url);
            notifier = (StreamConnectionNotifier)Connector.open(url);
        } catch (Exception e) {
            e.printStackTrace();
            return;
        }
                // waiting for connection
        while(true) {
            try {
                System.out.println("waiting for connection...");
                connection = notifier.acceptAndOpen();
                System.out.println("Client Connected...");
                Thread processThread = new Thread(new ProcessConnectionThread(connection));
                processThread.start();
            } catch (Exception e) {
                e.printStackTrace();
                return;
            }
        }
    }
}