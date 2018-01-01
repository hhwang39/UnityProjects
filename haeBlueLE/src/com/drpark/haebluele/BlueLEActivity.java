package com.drpark.haebluele;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGattCharacteristic;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class BlueLEActivity extends UnityPlayerActivity implements BluetoothLeUart.Callback {

    // UI elements

    // Bluetooth LE UART instance.  This is defined in BluetoothLeUart.java.
    private BluetoothLeUart uart;

    // Write some text to the messages text view.
    // Care is taken to do this on the main UI thread so writeLine can be called from any thread
    // (like the BTLE callback).
    private void writeLine(final String text) {
    	UnityPlayer.UnitySendMessage("BluetoothLE", "setSystemStr", text);
    }

    // Handler for mouse click on the send button.
    public void sendData(String message) {
    	UnityPlayer.UnitySendMessage("BluetoothLE", "setSystemStr", "Sending... " + message);
        StringBuilder stringBuilder = new StringBuilder();
        // We can only send 20 bytes per packet, so break longer messages
        // up into 20 byte payloads
        int len = message.length();
        int pos = 0;
        while(len != 0) {
        	UnityPlayer.UnitySendMessage("BluetoothLE", "setSystemStr", "Sending... len:" + String.valueOf(len));
            stringBuilder.setLength(0);
            if (len>=20) {
                stringBuilder.append(message.toCharArray(), pos, 20 );
                len-=20;
                pos+=20;
            }
            else {
                stringBuilder.append(message.toCharArray(), pos, len);
                len = 0;
            }
            UnityPlayer.UnitySendMessage("BluetoothLE", "setSystemStr", "Sending... builder:" + stringBuilder.toString());
            uart.send(stringBuilder.toString());
        }
        //UnityPlayer.UnitySendMessage("BluetoothLE", "setSystemStr", "exit while loop");
    }
    //start scanning
    public void startScan() {
    	writeLine("Scanning please wait...");
    	uart.registerCallback(this);
    	uart.connectFirstAvailable();
    }
    // when disconnect
    public void onClose() {
    	uart.unregisterCallback(this);
    	uart.disconnect();
    }
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }
    //create bluetoothleuart
    public void createUART() {
    	uart = new BluetoothLeUart(getApplicationContext());
    }
    public void connectToUART() {
    	uart.connectUART();
    }
    // UART Callback event handlers.
    @Override
    public void onConnected(BluetoothLeUart uart) {
        // Called when UART device is connected and ready to send/receive data.
        writeLine("Connected!");
        UnityPlayer.UnitySendMessage("BluetoothLE", "notifiedConnection", "");
        // Enable the send button
    }

    @Override
    public void onConnectFailed(BluetoothLeUart uart) {
        // Called when some error occured which prevented UART connection from completing.
        writeLine("Error connecting to device!");
        UnityPlayer.UnitySendMessage("BluetoothLE", "notifiedDisonnection", "");
    }

    @Override
    public void onDisconnected(BluetoothLeUart uart) {
        // Called when the UART device disconnected.
        writeLine("Disconnected!");
        UnityPlayer.UnitySendMessage("BluetoothLE", "notifiedDisonnection", "");
        // Disable the send button.
    }

    @Override
    public void onReceive(BluetoothLeUart uart, BluetoothGattCharacteristic rx) {
        // Called when data is received by the UART.
        //writeLine("Received: " + rx.getStringValue(0));
    	UnityPlayer.UnitySendMessage("BluetoothLE", "readData", rx.getStringValue(0));
    }

    @Override
    public void onDeviceFound(BluetoothDevice device) {
        // Called when a UART device is discovered (after calling startScan).
        //writeLine("Found device : " + device.getAddress());
    	addDevice(device.getAddress());
    	addDevice(device.getName());
        //writeLine("Waiting for a connection ...");
    }

    @Override
    public void onDeviceInfoAvailable() {
       // writeLine(uart.getDeviceInfo());
    }
    
    public void addDevice(String dev) {
    	UnityPlayer.UnitySendMessage("BluetoothLE", "AddDevice", dev);
    }
}