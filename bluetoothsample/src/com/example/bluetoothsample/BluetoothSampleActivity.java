package com.example.bluetoothsample;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Set;
import java.util.UUID;

import android.annotation.TargetApi;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.View;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
//This has been excerpted from http://www.atmarkit.co.jp/ait/articles/1107/29/news122.html
//Bluetooth
@TargetApi(Build.VERSION_CODES.GINGERBREAD)
public class BluetoothSampleActivity extends UnityPlayerActivity {
    private static final int FLAG_REQUEST_ENABLE_BT = 1;
    private static final int FLAG_BTEVENT_START_SEARCH = 1;
	public static final int DATA_RECEIVED = 3;
	public static final int SOCKET_CONNECTED = 4;
    private static BluetoothSampleActivity mBluetoothSampleActivity = null;
    private BluetoothAdapter mBluetoothAdapter = null;
    private BluetoothReceiver mBluetoothReceiver = null;
    private final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");      
    /** The arduino device name. */
    private final String arduinoDeviceName = "Prism";
    private Handler mCommandHandler = null;
    private ConnectedThread ct = null;
    private boolean isConnect = false;
    private BluetoothDevice dev = null;
    private String data;
    // ** Called when the activity is first created. * /
    @Override
    public void onCreate (Bundle savedInstanceState) {// [3]
        mBluetoothSampleActivity = this; // [4]
        mCommandHandler = new Handler () {
        	@Override
            public void handleMessage (Message message) {
                switch (message.what) {
                case FLAG_BTEVENT_START_SEARCH :
                    mBluetoothSampleActivity.checkBluetoothInternal ();
                    break;
            	case SOCKET_CONNECTED: {
            		ct = (ConnectedThread) message.obj;
            			if (!getConnected())
            				ct.write("this is a message".getBytes());
            			break;
            	}
            	case DATA_RECEIVED: {
            		data = (String) message.obj;
            		if (getConnected()) {
            			ct.write(data.getBytes());
            		}		
            	}
            		default:
            			break;
            	}
            }
        };
        super.onCreate (savedInstanceState);
    }
    private boolean getConnected() {
    	return isConnect;
    }
    private void setConnect(boolean con) {
    	isConnect = con;
    }
	public void startAsServer() {
		UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "starting server");
		new AcceptThread().start();
	}
	public void connectToServer() {
		UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "starting bluetooth connection");
		dev = getDevice();
		setConnect(true);
		if (dev == null) {
			UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Cannot find device");
		} else {
			UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Found device");
		}
		new ConnectThread(dev).start();		
	}
    private BluetoothDevice getDevice() {
        BluetoothAdapter local = BluetoothAdapter.getDefaultAdapter();
        BluetoothDevice theDevice = null;
        if (local != null) {
            if(!local.isEnabled()) {
                //Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
                //startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }
            while (theDevice == null) {
                Set<BluetoothDevice> pairedDevices = local.getBondedDevices();
                // If there are paired devices
                if (pairedDevices.size() > 0) {
                    // Loop through paired devices
                    for (BluetoothDevice device : pairedDevices) {
                        // Find the Arduino device
                        if(device.getName().equals(arduinoDeviceName))
                        {
                        	UnityPlayer.UnitySendMessage("CallJavaCode", "addBTDevice", device.getName());
                            theDevice = device;
                            break;
                        }
                    }
                }
            }
        }
        return theDevice;
    }
    public void write(String str) {
    	if (getConnected()) {
    		ct.write(str.getBytes());
    	}
    }
/*    public void getUUID() throws IllegalAccessException, IllegalArgumentException, InvocationTargetException, NoSuchMethodException {
        BluetoothAdapter adapter = BluetoothAdapter.getDefaultAdapter();

        Method getUuidsMethod = BluetoothAdapter.class.getDeclaredMethod("getUuids", null);

        ParcelUuid[] uuids = (ParcelUuid[]) getUuidsMethod.invoke(adapter, null);

        for (ParcelUuid uuid: uuids) {
        	//Log.d("BLUETOOTH", "UUID: " + uuid.getUuid().toString());
        	UnityPlayer.UnitySendMessage("CallJavaCode", "addUUID", uuid.getUuid().toString());
        }
    }*/
    @Override
    protected void onActivityResult (int requestCode, int resultCode, Intent data) {
        if (requestCode == FLAG_REQUEST_ENABLE_BT) {
            if (resultCode == RESULT_OK) {
                UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "BT Device has been turned on.");
                getLocalInformation ();
            } else if (resultCode == RESULT_CANCELED) {
                UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "BT Device is disabled.");
            }
        }
    }
 
    public void checkBluetooth () {
        mCommandHandler.sendMessage (Message.obtain (mCommandHandler, FLAG_BTEVENT_START_SEARCH, null));
    }
    private void checkBluetoothInternal () {
        mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter ();
        if (mBluetoothAdapter == null) {
            UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "BT Device Status : Not Supported.");
        } else {
            if (mBluetoothAdapter.isEnabled ()) {
                UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "BT Device Status : Enabled.");
                getLocalInformation ();
            } else {
                UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "BT Device Status : Disabled.");
                Intent enableBTIntent = new Intent (BluetoothAdapter.ACTION_REQUEST_ENABLE);
                startActivityForResult (enableBTIntent, FLAG_REQUEST_ENABLE_BT);
            }
        }
    }

    private void getLocalInformation () {
        UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "Step 2 : Checking own BT device scan mode ..."+ mBluetoothAdapter.getName () + ":"+ mBluetoothAdapter.getAddress ());
        switch (mBluetoothAdapter.getScanMode ()) {
        case BluetoothAdapter.SCAN_MODE_CONNECTABLE :
            Log.d ( "BLUETOOTH", "SCAN_MODE_CONNECTABLE");
            break;
        case BluetoothAdapter.SCAN_MODE_CONNECTABLE_DISCOVERABLE :
            Log.d ( "BLUETOOTH", "SCAN_MODE_CONNECTABLE_DISCOVERABLE");
            break;
        case BluetoothAdapter.SCAN_MODE_NONE :
            Log.d ( "BLUETOOTH", "SCAN_MODE_NONE");
            break;
        }
 
        switch (mBluetoothAdapter.getState ()) {
        case BluetoothAdapter.STATE_OFF :
            Log.d ( "BLUETOOTH", "STATE_OFF");
            break;
        case BluetoothAdapter.STATE_ON :
            Log.d ( "BLUETOOTH", "STATE_ON");
            break;
        case BluetoothAdapter.STATE_TURNING_OFF :
            Log.d ( "BLUETOOTH", "STATE_TURNING_OFF");
            break;
        case BluetoothAdapter.STATE_TURNING_ON :
            Log.d ( "BLUETOOTH", "STATE_TURNING_ON");
            break;
        }
        discoverDevices ();
    }
    private void discoverDevices () {
        UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "Step 4 : Searching Bluetooth devices ...");
        mBluetoothReceiver = new BluetoothReceiver ();
        registerReceiver (mBluetoothReceiver, new IntentFilter (BluetoothDevice.ACTION_FOUND));
        mBluetoothAdapter.startDiscovery ();
    }
	public void manageConnectedSocket(BluetoothSocket socket) {
		ct = new ConnectedThread(socket);
		mCommandHandler.obtainMessage(
				SOCKET_CONNECTED, ct).sendToTarget();
		ct.start();
	}
    private class BluetoothReceiver extends BroadcastReceiver {
        @Override
        public void onReceive (Context context, Intent intent) {
            UnityPlayer.UnitySendMessage ( "CallJavaCode", "BluetoothMessage", "Found Bluetooth devices.");
            String action = intent.getAction ();
            if (BluetoothDevice.ACTION_FOUND.equals (action)) {
                BluetoothDevice device = intent
                .getParcelableExtra (BluetoothDevice.EXTRA_DEVICE);
                UnityPlayer.UnitySendMessage ( "CallJavaCode", "addBTDevice", device.getName () + "["+ device.getAddress () + "]");
            }
        }
    }
    public void off(View view) {
    	mBluetoothAdapter.disable();
    	ct.cancel();
    	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Disconnected");
    }
    @Override
    protected void onDestroy() {
    	super.onDestroy();
    	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Destroyed");
    	unregisterReceiver(mBluetoothReceiver);
    }
	/**
	 * The Class ConnectedThread.
	 */
	private class ConnectThread extends Thread {
        
        /** The arduino socket. */
        private final BluetoothSocket arduinoSocket;
        
        /** The arduino. */
        private final BluetoothDevice arduino;

        /**
         * Instantiates a new connected thread.
         *
         * @param device the device
         */
        public ConnectThread(BluetoothDevice device) {
            BluetoothSocket tmp = null;
        	//OutputStream tmpOut = null;
        	//InputStream tmpIn = null;

        	arduino = device;
            // Get the input and output streams, using temp objects because
            // member streams are final
            try {
                tmp = arduino.createRfcommSocketToServiceRecord(MY_UUID);
                //tmpOut = tmp.getOutputStream();
            } catch (IOException e) { }
            arduinoSocket = tmp;
            //mmOutStream = tmpOut;
            //mmInStream = tmpIn;

        }
     
        /**
         * Starts the thread
         * 
         * @see java.lang.Thread#run()
         */
        public void run() {
        	//mBluetoothAdapter.cancelDiscovery();
        	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "running run()");            
            try {
                // Connect the device through the socket. This will block
                // until it succeeds or throws an exception
                arduinoSocket.connect();
                UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "connecting run()");  
                manageConnectedSocket(arduinoSocket);
            } catch (IOException connectException) {
                // Unable to connect; close the socket and get out
                try {
                	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "can't connect");
                    arduinoSocket.close();
                    
                } catch (IOException closeException) { }
            }
            //while(true) { }
        }

        
        /**
         * Cancel.
         * Call this from the main Activity to shutdown the connection
         */
        public void cancel() {
            try {
                arduinoSocket.close();
            } catch (IOException e) { 
            	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Error Closing");
            }
        }
        
    }
	private class AcceptThread extends Thread {
		BluetoothAdapter mmBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
	    private final BluetoothServerSocket mmServerSocket;
        private BluetoothSocket socket = null;
	    public AcceptThread() {
	        // Use a temporary object that is later assigned to mmServerSocket,
	        // because mmServerSocket is final
	        BluetoothServerSocket tmp = null;
	        try {
	            // MY_UUID is the app's UUID string, also used by the client code
	        	if (mmBluetoothAdapter == null) UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "mBlutooth Null");
	            tmp = mmBluetoothAdapter.listenUsingRfcommWithServiceRecord("GTZzang", MY_UUID);
	        } catch (IOException e) { }
	        mmServerSocket = tmp;
	    }
	 
	    public void run() {
	        // Keep listening until exception occurs or a socket is returned
	        while (true) {
	            try {
	            	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Server Accepting");
	                socket = mmServerSocket.accept();        
		                // Do work to manage the connection (in a separate thread)
		            manageConnectedSocket(socket);
		            mmServerSocket.close();
		            break;
	            } catch (IOException e) {
	            	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Server Not Accepting");
	                break;
	            }
	            // If a connection was accepted

	        }
	    }
	 
	    /** Will cancel the listening socket, and cause the thread to finish */
	    public void cancel() {
	        try {
	            mmServerSocket.close();
	        } catch (IOException e) { }
	    }
	}
	private class ConnectedThread extends Thread {
	    private final BluetoothSocket mmSocket;
	    private final InputStream mmInStream;
	    private final OutputStream mmOutStream;
	 
	    public ConnectedThread(BluetoothSocket socket) {
	        mmSocket = socket;
	        InputStream tmpIn = null;
	        OutputStream tmpOut = null;
	 
	        // Get the input and output streams, using temp objects because
	        // member streams are final
	        try {
	            tmpIn = socket.getInputStream();
	            tmpOut = socket.getOutputStream();
	        } catch (IOException e) { 
	        	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "Sry null socket");
	        }
	 
	        mmInStream = tmpIn;
	        mmOutStream = tmpOut;
	    }
	 
	    public void run() {
	        byte[] buffer = new byte[1024];  // buffer store for the stream
	        int bytes; // bytes returned from read()
	        
	        // Keep listening to the InputStream until an exception occurs
	        while (true) {
	            try {
	            	UnityPlayer.UnitySendMessage("CallJavaCode", "BluetoothMessage", "running ConnectedThread");
	                // Read from the InputStream
	            	bytes = mmInStream.read(buffer);
	            	String data = new String(buffer, 0, bytes);
	    			mCommandHandler.obtainMessage(
	    					DATA_RECEIVED, data).sendToTarget();
	    			UnityPlayer.UnitySendMessage("CallJavaCode", "setMSG", data);
	            } catch (IOException e) {
	                break;
	            }
	        }
	    }
	 
	    /* Call this from the main activity to send data to the remote device */
	    public void write(byte[] bytes) {
	        try {
	            mmOutStream.write(bytes);
	        } catch (IOException e) { }
	    }
	 
	    /* Call this from the main activity to shutdown the connection */
	    public void cancel() {
	        try {
	            mmSocket.close();
	        } catch (IOException e) { }
	    }
	}
} 
