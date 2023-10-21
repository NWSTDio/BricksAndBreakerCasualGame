public class GeometryOperation {

	public static double vectorCrossProduct(Vector vector1, Vector vector2) {
		return vector1.getxComponent() * vector2.getyComponent() - vector2.getxComponent() * vector1.getyComponent();
	}

	public static boolean rangeIntersection(double a, double b, double c, double d) {
		if (a > b) {
			double temp = a;
			a = b;
			b = temp;
		}
		if (c > d) {
			double temp = c;
			c = d;
			d = temp;
		}
		return Math.max(a, c) <= Math.min(b, d);
	}

	public static boolean boundingBox(Segment ab, Segment cd) {
		boolean xRangeIntersection = rangeIntersection(ab.getStartPoint().getX(), ab.getEndPoint().getX(),
				cd.getStartPoint().getX(), cd.getEndPoint().getX());
		boolean yRangeIntersection = rangeIntersection(ab.getStartPoint().getY(), ab.getEndPoint().getY(),
				cd.getStartPoint().getY(), cd.getEndPoint().getY());
		return xRangeIntersection && yRangeIntersection;
	}

	public static boolean checkSegmentIntersection(Segment ab, Segment cd) {
		if (!boundingBox(ab, cd)) {
			return false;
		}
		Vector vAB = new Vector(ab.getStartPoint(), ab.getEndPoint());
		Vector vAC = new Vector(ab.getStartPoint(), cd.getStartPoint());
		Vector vAD = new Vector(ab.getStartPoint(), cd.getEndPoint());

		Vector vCD = new Vector(cd.getStartPoint(), cd.getEndPoint());
		Vector vCA = new Vector(cd.getStartPoint(), ab.getStartPoint());
		Vector vCB = new Vector(cd.getStartPoint(), ab.getEndPoint());

		double d1 = vectorCrossProduct(vAB, vAC);
		double d2 = vectorCrossProduct(vAB, vAD);

		double d3 = vectorCrossProduct(vCD, vCA);
		double d4 = vectorCrossProduct(vCD, vCB);
		return ((d1 <= 0 && d2 >= 0) || (d1 >= 0 && d2 <= 0)) && ((d3 <= 0 && d4 >= 0) || (d3 >= 0 && d4 <= 0));
	}

}
