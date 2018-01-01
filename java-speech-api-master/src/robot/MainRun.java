package robot;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

import com.darkprograms.speech.recognizer.RecognizerChunked;

public class MainRun {
	public static void main (String args[]) {
		System.out.println("No worries");
		HaeRobot HR = new HaeRobot();
		HR.start();
		while (!HR.getBool()) {};
		try {
			run();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			System.out.println("ERROR DETECTED: " + e.toString());
		}
	}
	public static void run() throws IOException {
		RecognizerChunked regChunk = new RecognizerChunked("AIzaSyBBRJlz7w1ZnKQPvQJf_cPGHVDjCLh1QU0");
		Path path = Paths.get("src/k.flac");
		byte[] data = Files.readAllBytes(path);
		regChunk.getRecognizedDataForFlac(data, 44100);
		System.out.println("Success");	
	}
}
