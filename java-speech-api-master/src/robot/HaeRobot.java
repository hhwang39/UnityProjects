package robot;

import java.io.File;

import javaFlacEncoder.FLAC_FileEncoder;

//A wrapper class that does everything
public class HaeRobot {
	private boolean done;
	public HaeRobot() {
		done = false;
	}
    /**
     * A wrapper that will record and convert it to flac file.
     */
	public void start() {
		SoundRecordWrapper srw = new SoundRecordWrapper("k.wav");
		srw.record();
		while (!srw.isFinish()) {};
		FLAC_FileEncoder ffe = new FLAC_FileEncoder();
		ffe.encode(new File("src/k.wav"), new File("src/k.flac"));
		System.out.println("Done");
		done = true;
	}
	public boolean getBool() {
		return done;
	}
}
