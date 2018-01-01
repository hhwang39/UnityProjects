package robot;

public class Darwin {
	public Darwin() {
		
	}
	public static String responseDarwin(String str) {
		int ind = str.indexOf("Darwin");
		if (ind != -1) {
			return "Darwin: Yes Ma'am I will start doing my work";
		}
		return quesitonDarwin();
	}
	private static String quesitonDarwin() {
		return "Darwin: You are not talking to me right ma'am?";
	}
}
