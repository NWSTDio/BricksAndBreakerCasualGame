public class Main {

	public static void main(String[] args) {

		// точки
		Point a = new Point(1, 2);
		Point b = new Point(3, 1);
		Point c = new Point(1, 1);
		Point d = new Point(4, 3);

		// отрезки
		Segment ab = new Segment(a, b);
		Segment cd = new Segment(c, d);

		if (GeometryOperation.checkSegmentIntersection(ab, cd)) {
			System.out.println("Intersect");
		} else {
			System.out.println("Not intersect");
		}

	}

}
