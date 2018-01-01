package server;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Scanner;

import javax.microedition.io.StreamConnection;

public class ProcessConnectionThread implements Runnable {

    private StreamConnection mConnection;
    private OutputStream outStream;
    private Scanner mScan;
    // Constant that indicate command from devices
    private static final int EXIT_CMD = -1;
    private static final int KEY_RIGHT = 1;
    private static final int KEY_LEFT = 2;
    private static final String SIGNAL_WRITE = ";";
    public ProcessConnectionThread(StreamConnection connection)
    {
        mConnection = connection;
        try {
        	mScan = new Scanner(System.in);
			outStream = mConnection.openOutputStream();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        
    }

    @Override
    public void run() {
        try {
            // prepare to receive data
            InputStream inputStream = mConnection.openInputStream();
            System.out.println("waiting for input");
        	byte[] bytes = new byte[1024];
            while (true) {

                int command = inputStream.read(bytes);
                String data = new String(bytes, 0, command);
                if (command == EXIT_CMD) {
                    System.out.println("finish process");
                    break;
                }
                if (data.equalsIgnoreCase(";")) {
                	System.out.println("Type your response");
                	String response = mScan.nextLine();
                	write(response.getBytes());
                	
                }
                processCommand(data);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    public void write(byte[] bytes) {
    	try {
			outStream.write(bytes);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }

    /**
     * Process the command from client
     * @param command the command code
     */
    private void processCommand(String data) {
    	System.out.println(data);
    }
}