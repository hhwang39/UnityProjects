package robot;

import java.awt.FlowLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JFrame;

public class SoundRecordWrapper {
	private final SoundRecorder recorder;
	private String str;
    boolean finishBool;
    //make gui to click on it
	private JButton start, stop;
	private JFrame frame;
	public SoundRecordWrapper(String name) {
		ImageIcon ic1 = new ImageIcon("src/record-button.png");
		ImageIcon ic2 = new ImageIcon("src/index.jpg");
		start = new JButton(ic1);
		stop = new JButton(ic2);
		start.setSize(100, 100);
		stop.setSize(100, 100);
    	finishBool = false;
		str = name;
		initGUI();
		recorder = new SoundRecorder(str);
	}
	private void initGUI() {
		start.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				Thread starter = new Thread(new Runnable() {
					@Override
					public void run() {
						// TODO Auto-generated method stub
						recorder.start();
					}
				});
				// TODO Auto-generated method stub
				starter.start();
			}
		});
		stop.addActionListener(new ActionListener() {		
			@Override
			public void actionPerformed(ActionEvent e) {
				recorder.finish();
				frame.dispose();
				finishBool = true;
			}
		});
	}
	public void record() {
		frame = new JFrame("Record");
		frame.setSize(600, 400);
		frame.setLayout(new FlowLayout());
		frame.add(start);
		frame.add(stop);
		frame.setVisible(true);
		//System.out.println("How long would you like to record in s? must be between 1 - 15");
		//Scanner sc = new Scanner(System.in);
		//long t = sc.nextLong();
		//t = t * 1000;
		//recordWrap(t, str);
	}
/*	public void recordWrap(final long time, String fileName) {
	       
	        // creates a new thread that waits for a specified
	        // of time before stopping
	        Thread stopper = new Thread(new Runnable() {
	            public void run() {
	                try {
	                    Thread.sleep(time);
	                } catch (InterruptedException ex) {
	                    ex.printStackTrace();
	                }
	                recorder.finish();   
	                finishBool = true;
	            }	
	        });
	 
	        stopper.start();
	 
	        // start recording
	        recorder.start();
	}*/
    public boolean isFinish() {
    	return finishBool;
    }
}
