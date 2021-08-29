public static class TrackingPoints
{
    //public static int[] eyePointsTop = { 156, 46, 53, 52, 65, 55, 193, 122, 244, 243 };
    //public static int[] eyePointsTop = { 55, 107, 66, 105, 63, 70, 53, 52, 65 };
    //public static int[] eyePointsTop = { 193, 55, 65, 52, 53, 70, 46, 224, 223, 222, 221 };
    public static int[] eyePointsTop = { 156, 46, 53, 52, 65, 55, 193, 244, 243, 225, 224, 223, 222, 221, 193, 245, 244, 243 };

    //public static int[] eyePointsMid = { 113, 225, 224, 223, 222, 221, 193, 245, 244, 243 };
    //public static int[] eyePointsMid = { 221, 222, 223, 224, 225, 46, 56, 28, 27, 29, 30, 193, 189, 156 };
    //public static int[] eyePointsMid = { 189, 221, 222, 223, 224, 46, 225, 30, 29, 27, 28, 56 };
    public static int[] eyePointsMid = { 113, 225, 224, 223, 222, 221, 193, 245, 244, 243, 173, 157, 158, 159, 160, 161, 246, 226 };

    //public static int[] eyePointsBot = { 113, 30, 29, 27, 28, 56, 189, 245, 244, 243 };
    //public static int[] eyePointsBot = { 190, 243, 245, 124, 247, 470};
    public static int[] eyePointsBot = { 190, 56, 28, 27, 29, 30, 226, 246, 161, 160, 159, 158, 157, 173, 243, 244 };

    public static int[] eyePointsUnd = { 163, 144, 145, 153, 154, 155, 133, 243, 244, 233, 232, 231, 230, 229, 228, 31, 124, 113, 247, 246 };
    //public static int[] eyePointsUnd = { 226, 163, 144, 145, 153, 154, 155, 133, 243, 244, 233, 232, 231, 230, 229, 228, 31 };

    public static int[] eyePointsLiner = { 133, 155, 154, 153, 145, 144, 163, 7, 33, 130 };
    //public static int[] eyePointsLiner = { 173, 157, 158, 159, 160, 161 };
    //public static int[] eyePointsLiner = { 244, 243, 173, 157, 158, 159, 160, 161, 246, 226 };

    //public static int[] eyePointsTop2 = { 464, 465, 351, 417, 285, 295, 282, 283, 276, 353, 265, 446 };
    public static int[] eyePointsTop2 = { 464, 465, 351, 417, 285, 295, 282, 283, 276, 353, 265, 446, 342, 445, 444, 443, 442, 441, 413 };

    //public static int[] eyePointsMid2 = { 465, 413, 441, 442, 443, 444, 445, 342, 446 };
    //public static int[] eyePointsMid2 = { 465, 413, 441, 442, 443, 444, 445, 342, 446, 467, 260, 259, 257, 258, 286, 413, 414 };
    public static int[] eyePointsMid2 = { 465, 413, 441, 442, 443, 444, 445, 342, 467, 446, 466, 388, 387, 386, 385, 384, 398, 463 };

    //public static int[] eyePointsBot2 = { 464, 465, 413, 286, 258, 257, 259, 260, 467, 446 };
    //public static int[] eyePointsBot2 = { 398, 384, 385, 386, 387, 388, 466, 446, 467, 475, 414 };
    public static int[] eyePointsBot2 = { 465, 463, 398, 384, 385, 386, 387, 388, 466, 446, 467, 260, 259, 257, 258, 286, 413, 414 };

    public static int[] eyePointsUnd2 = { 465, 453, 452, 451, 450, 449, 448, 261, 265, 353, 342, 467, 466, 390, 373, 374, 380, 381, 382, 362 };

    public static int[] eyePointsLiner2 = { 263, 249, 390, 373, 374, 380, 381, 382, 362, 463 };

    public static int[][] eyePoints = {
        eyePointsTop,
        eyePointsMid,
        eyePointsBot,
        eyePointsUnd,
        eyePointsTop2,
        eyePointsMid2,
        eyePointsBot2,
        eyePointsUnd2
    };

    public static int[][] linerPoints = {
        eyePointsLiner, eyePointsLiner2
    };
}
