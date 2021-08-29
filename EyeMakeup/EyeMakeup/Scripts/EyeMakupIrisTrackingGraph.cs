using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using Mediapipe;
using OpenCvSharp;

public class EyeMakupIrisTrackingGraph : DemoGraph
{
    private const string faceLandmarksWithIrisStream = "face_landmarks_with_iris";
    private OutputStreamPoller<NormalizedLandmarkList> faceLandmarksWithIrisStreamPoller;
    private NormalizedLandmarkListPacket faceLandmarksWithIrisPacket;

    private const string faceRectStream = "face_rect";
    private OutputStreamPoller<NormalizedRect> faceRectStreamPoller;
    private NormalizedRectPacket faceRectPacket;

    private const string faceDetectionsStream = "face_detections";
    private OutputStreamPoller<List<Detection>> faceDetectionsStreamPoller;
    private DetectionVectorPacket faceDetectionsPacket;

    private const string faceLandmarksWithIrisPresenceStream = "face_landmarks_with_iris_presence";
    private OutputStreamPoller<bool> faceLandmarksWithIrisPresenceStreamPoller;
    private BoolPacket faceLandmarksWithIrisPresencePacket;

    private const string faceDetectionsPresenceStream = "face_detections_presence";
    private OutputStreamPoller<bool> faceDetectionsPresenceStreamPoller;
    private BoolPacket faceDetectionsPresencePacket;

    private EyeMakupParams makeupParams;

    private Point[][] previousFilters;
    private Point[][] previousLiners;


    public override Status StartRun()
    {
        faceLandmarksWithIrisStreamPoller = graph.AddOutputStreamPoller<NormalizedLandmarkList>(faceLandmarksWithIrisStream).Value();
        faceLandmarksWithIrisPacket = new NormalizedLandmarkListPacket();

        faceRectStreamPoller = graph.AddOutputStreamPoller<NormalizedRect>(faceRectStream).Value();
        faceRectPacket = new NormalizedRectPacket();

        faceDetectionsStreamPoller = graph.AddOutputStreamPoller<List<Detection>>(faceDetectionsStream).Value();
        faceDetectionsPacket = new DetectionVectorPacket();

        faceLandmarksWithIrisPresenceStreamPoller = graph.AddOutputStreamPoller<bool>(faceLandmarksWithIrisPresenceStream).Value();
        faceLandmarksWithIrisPresencePacket = new BoolPacket();

        faceDetectionsPresenceStreamPoller = graph.AddOutputStreamPoller<bool>(faceDetectionsPresenceStream).Value();
        faceDetectionsPresencePacket = new BoolPacket();

        makeupParams = GetComponent<EyeMakupParams>();
        if (makeupParams == null)
        {
            throw new MissingComponentException("EyeMakupParams component is required");
        }

        previousFilters = new Point[TrackingPoints.eyePoints.Length][];
        previousLiners = new Point[TrackingPoints.linerPoints.Length][];

        return graph.StartRun();
    }

    public override void RenderOutput(WebCamScreenController screenController, TextureFrame textureFrame)
    {
        var faceMeshValue = FetchNextIrisTrackingValue();
        Texture2D output = RenderEyeMakeup(screenController, textureFrame, faceMeshValue);
        if (output != null)
        {
            screenController.DrawScreen(output.GetPixels32());
        }
        else
        {
            screenController.DrawScreen(textureFrame);
        }
    }

    private IrisTrackingValue FetchNextIrisTrackingValue()
    {
        if (!FetchNextFaceLandmarksWithIrisPresence())
        {
            // face not found
            return new IrisTrackingValue();
        }

        var multiFaceLandmarks = FetchNextFaceLandmarksWithIris();
        var faceRects = FetchNextFaceRect();

        if (!FetchNextFaceDetectionsPresence())
        {
            return new IrisTrackingValue(multiFaceLandmarks, faceRects);
        }

        var faceDetections = FetchNextFaceDetections();

        return new IrisTrackingValue(multiFaceLandmarks, faceRects, faceDetections);
    }

    private bool FetchNextFaceLandmarksWithIrisPresence()
    {
        return FetchNext(faceLandmarksWithIrisPresenceStreamPoller, faceLandmarksWithIrisPresencePacket, faceLandmarksWithIrisPresenceStream);
    }

    private NormalizedLandmarkList FetchNextFaceLandmarksWithIris()
    {
        return FetchNext(faceLandmarksWithIrisStreamPoller, faceLandmarksWithIrisPacket, faceLandmarksWithIrisStream);
    }

    private NormalizedRect FetchNextFaceRect()
    {
        return FetchNext(faceRectStreamPoller, faceRectPacket, faceRectStream);
    }

    private bool FetchNextFaceDetectionsPresence()
    {
        return FetchNext(faceDetectionsPresenceStreamPoller, faceDetectionsPresencePacket, faceDetectionsPresenceStream);
    }

    private List<Detection> FetchNextFaceDetections()
    {
        return FetchNextVector(faceDetectionsStreamPoller, faceDetectionsPacket, faceDetectionsStream);
    }

    protected override void PrepareDependentAssets()
    {
        PrepareDependentAsset("face_detection_short_range.bytes");
        PrepareDependentAsset("face_landmark.bytes");
        PrepareDependentAsset("iris_landmark.bytes");
    }

    private Mat TextureFrameToMat(TextureFrame textureFrame)
    {
        int width = textureFrame.width;
        int height = textureFrame.height;

        Color32[] pixels = textureFrame.GetPixels32();

        Vec3b[] imageData = new Vec3b[width * height];

        Parallel.For(0, height, i =>
        {
            for (int j = 0; j < width; ++j)
            {
                int currentPos = i * width + j;
                Color32 pixel = pixels[currentPos];
                imageData[currentPos] = new Vec3b
                {
                    Item0 = pixel.b,
                    Item1 = pixel.g,
                    Item2 = pixel.r
                };
            }
        });

        Mat mat = new Mat(height, width, MatType.CV_8UC3, imageData);
        if (makeupParams.flipImage)
        {
            mat = mat.Flip(FlipMode.Y);
        }
        return mat;
    }

    private Texture2D MatToTexture(Mat mat)
    {
        int width = mat.Width;
        int height = mat.Height;

        Vec3b[] imageData = new Vec3b[width * height];
        mat.GetArray(0, 0, imageData);

        Color32[] pixels = new Color32[width * height];

        Parallel.For(0, height, i =>
        {
            for (int j = 0; j < width; ++j)
            {
                int currentPos = i * width + j;
                Vec3b current = imageData[currentPos];
                pixels[currentPos] = new Color32
                {
                    b = current[0],
                    g = current[1],
                    r = current[2],
                    a = 0
                };
            }
        });

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels32(pixels);
        texture.Apply();

        return texture;
    }

    private Texture2D RenderEyeMakeup(
        WebCamScreenController screenController,
        TextureFrame textureFrame,
        IrisTrackingValue value
        )
    {
        Mat image = TextureFrameToMat(textureFrame);

        if (value.FaceRect == null || value.FaceLandmarksWithIris == null || !makeupParams.applyMakeup)
        {
            return MatToTexture(image);
        }

        int imageWidth = textureFrame.width;
        int imageHeight = textureFrame.height;

        Point[] landmarks = new Point[value.FaceLandmarksWithIris.Landmark.Count];
        for (int i = 0; i < value.FaceLandmarksWithIris.Landmark.Count; ++i)
        {
            var landmark = value.FaceLandmarksWithIris.Landmark[i];
            int x = Convert.ToInt32(
                Mathf.Min(
                    Mathf.Floor(
                        (makeupParams.flipImage ? landmark.X : 1 - landmark.X) * imageWidth),
                    imageWidth - 1));
            int y = Convert.ToInt32(
                Mathf.Min(
                    Mathf.Floor(
                        (1 - landmark.Y) * imageHeight),
                    imageHeight - 1));
            landmarks[i] = new Point(x, y);
        }

        if (makeupParams.drawLandmarks)
        {
            image = image.Flip(FlipMode.X);
            for (int i = 0; i < landmarks.Length; ++i)
            //foreach (int i in TrackingPoints.eyePointsMid2)
            {
                //if (i < 400)
                //{
                //    continue;
                //}
                //if (landmarks[i].Y <= landmarks[245].Y)
                //{
                //    continue;
                //}
                Point point = new Point(landmarks[i].X, imageHeight - landmarks[i].Y);
                Cv2.Circle(image, point, 3, Scalar.Green);
                Cv2.PutText(image, i.ToString(), point, HersheyFonts.HersheyPlain, 0.8, Scalar.Red);
            }

            image = image.Flip(FlipMode.X);

            return MatToTexture(image);
        }

        for (int eyePointsIndex = 0; eyePointsIndex < TrackingPoints.eyePoints.Length; ++eyePointsIndex)
        {
            int[] eyePointIndexes = TrackingPoints.eyePoints[eyePointsIndex];
            int lH = makeupParams.GetH(eyePointsIndex);
            int lS = makeupParams.GetS(eyePointsIndex);
            int lV = makeupParams.GetV(eyePointsIndex);
            int blending = makeupParams.GetBlending(eyePointsIndex);
            int borderSmooth = makeupParams.GetBorderSmooth(eyePointsIndex);

            Point[] eyePoints = new Point[eyePointIndexes.Length];
            
            for (int i = 0; i < eyePointIndexes.Length; ++i)
            {
                eyePoints[i] = landmarks[eyePointIndexes[i]];
            }
            if (previousFilters[eyePointsIndex] == null)
            {
                previousFilters[eyePointsIndex] = eyePoints;
            }
            else
            {
                for (int i = 0; i < eyePoints.Length; ++i)
                {
                    int x = Convert.ToInt32(
                        (1 - makeupParams.filterStability) * eyePoints[i].X
                        + makeupParams.filterStability * previousFilters[eyePointsIndex][i].X);
                    int y = Convert.ToInt32(
                        (1 - makeupParams.filterStability) * eyePoints[i].Y
                        + makeupParams.filterStability * previousFilters[eyePointsIndex][i].Y);
                    eyePoints[i] = new Point(x, y);
                }

                previousFilters[eyePointsIndex] = eyePoints;
            }
            try
            {
                for (int i = 0; i < eyePointIndexes.Length; ++i)
                {
                    if (eyePoints[i].X < 0 || eyePoints[i].X >= imageWidth
                        || eyePoints[i].Y < 0 || eyePoints[i].Y >= imageHeight)
                    {
                        throw new ApplicationException();
                    }
                }
            }
            catch (ApplicationException)
            {
                continue;
            }

            OpenCvSharp.Rect roiRect = Cv2.BoundingRect(eyePoints);
            // roiImage is pointing to image data
            Mat roiImage = image[
                new Range(roiRect.Top, roiRect.Bottom + 1),
                new Range(roiRect.Left, roiRect.Right + 1)];

            // coloredRoiImage is u24 ROI that stores the original image blended with the filter HSV color
            Mat coloredRoiImage = new Mat(
                roiRect.Height,
                roiRect.Width,
                MatType.CV_8UC3,
                new Scalar(lH / 2, lS * 255 / 100, lV * 255 / 100));
            Cv2.CvtColor(
                src: coloredRoiImage,
                dst: coloredRoiImage,
                code: ColorConversionCodes.HSV2BGR);

            float p = blending / 100f;
            float q = 1 - p;
            Cv2.AddWeighted(
                src1: coloredRoiImage,
                alpha: p,
                src2: roiImage,
                beta: q,
                gamma: 0,
                dst: coloredRoiImage) ;

            // eyePointsMask is u8 ROI that stores 255 for eyePoints regions and 0 elsewhere
            Mat eyePointsMask = new Mat(
                imageHeight,
                imageWidth,
                MatType.CV_8UC1,
                new Scalar(0));
            Cv2.DrawContours(
                image: eyePointsMask,
                contours: new Point[][] { eyePoints },
                contourIdx: -1,
                color: new Scalar(255),
                thickness: -1);
            eyePointsMask = eyePointsMask[
                new Range(roiRect.Top, roiRect.Bottom + 1),
                new Range(roiRect.Left, roiRect.Right + 1)];

            // notEyePointsMask is u8 ROI that stores 0 for eyePoints regions and 255 elsewhere
            Mat notEyePointsMask = eyePointsMask.Clone();
            Cv2.BitwiseNot(src: eyePointsMask, dst: notEyePointsMask);

            // faceBef is u24 ROI that has colored pixels from the original image everywhere but for the eyePoints regions
            Mat faceBef = new Mat(
                roiRect.Height,
                roiRect.Width,
                MatType.CV_8UC3,
                new Scalar(0, 0, 0));
            Cv2.BitwiseAnd(
                src1: roiImage,
                src2: roiImage,
                dst: faceBef,
                mask: notEyePointsMask);

            // buffer is needed because Cv2.BilateralFilter cannot have the same src and dst
            Mat buffer = new Mat(
                roiRect.Height,
                roiRect.Width,
                MatType.CV_8UC3,
                new Scalar(0, 0, 0));
            Cv2.BilateralFilter(
                src: coloredRoiImage,
                dst: buffer,
                d: 15,
                sigmaColor: 47,
                sigmaSpace: 47);
            coloredRoiImage = buffer;
            Mat resultEyeFilter = coloredRoiImage.Clone();


            for (int j = borderSmooth; j > 0; j -= makeupParams.borderDecrement)
            {
                p = j / (float)borderSmooth;
                q = 1 - p;

                // borderLayerRoiImage is u24 ROI that stores the original image blured with the filter color
                Mat borderLayerRoiImage = new Mat(
                    roiRect.Height,
                    roiRect.Width,
                    MatType.CV_8UC3,
                    new Scalar(0, 0, 0));
                Cv2.AddWeighted(
                    src1: coloredRoiImage,
                    alpha: p,
                    src2: roiImage,
                    beta: q,
                    gamma: 0,
                    dst: borderLayerRoiImage);

                // eyePointsThickenedMask is u8 ROI that stores 255 for eyePoints contours + thickness and 0 elsewhere
                Mat eyePointsThickenedMask = new Mat(
                    imageHeight,
                    imageWidth,
                    MatType.CV_8UC1,
                    new Scalar(0));
                Cv2.DrawContours(
                    image: eyePointsThickenedMask,
                    contours: new Point[][] { eyePoints },
                    contourIdx: -1,
                    color: new Scalar(255),
                    thickness: j);
                eyePointsThickenedMask = eyePointsThickenedMask[
                    new Range(roiRect.Top, roiRect.Bottom + 1),
                    new Range(roiRect.Left, roiRect.Right + 1)];

                // notEyePointsThickenedMask is u8 ROI that stores 0 for eyePoints contours + thickness and 255 elsewhere
                Mat notEyePointsThickenedMask = eyePointsThickenedMask.Clone();
                Cv2.BitwiseNot(src: eyePointsThickenedMask, dst: notEyePointsThickenedMask);

                // temp2 is only needed to avoid writing into temp directly so that not-masked regions contain 0
                Mat temp2 = new Mat(
                    roiRect.Height,
                    roiRect.Width,
                    MatType.CV_8UC3,
                    new Scalar(0, 0, 0));
                // Preserve parts of borderLayerRoiImage that store eyePoints contours + thickness, 0 elsewhere
                Cv2.BitwiseAnd(
                    src1: borderLayerRoiImage,
                    src2: borderLayerRoiImage,
                    dst: temp2,
                    mask: eyePointsThickenedMask);

                // afaceBef is u24 ROI that stores original image + filter color everywhere but the eyePoints contours + thickness
                Mat afaceBef = new Mat(
                    roiRect.Height,
                    roiRect.Width,
                    MatType.CV_8UC3,
                    new Scalar(0, 0, 0));
                Cv2.BitwiseAnd(
                    src1: resultEyeFilter,
                    src2: resultEyeFilter,
                    dst: afaceBef,
                    mask: notEyePointsThickenedMask);

                // Combine eyePoints contours + thickness from temp2 and original image + filter color from afaceBef
                //resultRoiImage = resultRoiImage.Clone();
                Cv2.BitwiseOr(
                    src1: afaceBef,
                    src2: temp2,
                    dst: resultEyeFilter);
            }

            // temp3 is u24 ROI that stores eyePoints region after blending with the original image
            Mat temp3 = new Mat(
                roiRect.Height,
                roiRect.Width,
                MatType.CV_8UC3,
                new Scalar(0, 0, 0));
            Cv2.BitwiseAnd(
                src1: resultEyeFilter,
                src2: resultEyeFilter,
                dst: temp3,
                mask: eyePointsMask);

            // Combine the original image with eyePoints regions after blending with the original image
            Cv2.BitwiseOr(
                src1: faceBef,
                src2: temp3,
                dst: roiImage);
        }

        Mat linerMask = new Mat(
            imageHeight,
            imageWidth,
            MatType.CV_8UC1,
            new Scalar(0));

        for (int linerPointsIndex = 0; linerPointsIndex < TrackingPoints.linerPoints.Length; ++linerPointsIndex)
        {
            Point[] linerPoints = new Point[TrackingPoints.linerPoints[linerPointsIndex].Length];
            
            for (int i = 0; i < TrackingPoints.linerPoints[linerPointsIndex].Length; ++i)
            {
                linerPoints[i] = landmarks[TrackingPoints.linerPoints[linerPointsIndex][i]];
            }

            if (previousLiners[linerPointsIndex] == null)
            {
                previousLiners[linerPointsIndex] = linerPoints;
            }
            else
            {
                for (int i = 0; i < linerPoints.Length; ++i)
                {
                    int x = Convert.ToInt32(
                        (1 - makeupParams.linerStability) * linerPoints[i].X
                        + makeupParams.linerStability * previousLiners[linerPointsIndex][i].X);
                    int y = Convert.ToInt32(
                        (1 - makeupParams.linerStability) * linerPoints[i].Y
                        + makeupParams.linerStability * previousLiners[linerPointsIndex][i].Y);
                    linerPoints[i] = new Point(x, y);
                }

                previousLiners[linerPointsIndex] = linerPoints;
            }

            try
            {
                for (int i = 0; i < linerPoints.Length; ++i)
                {
                    if (linerPoints[i].X < 0 || linerPoints[i].X >= imageWidth
                        || linerPoints[i].Y < 0 || linerPoints[i].Y >= imageHeight)
                    {
                        throw new ApplicationException();
                    }
                }
            }
            catch (ApplicationException)
            {
                continue;
            }

            Cv2.Polylines(
                img: linerMask,
                new Point[][] { linerPoints },
                isClosed: false,
                color: new Scalar(255),
                thickness: makeupParams.linerThickness);
        }
        Mat kernel = Cv2.GetStructuringElement(
            shape: MorphShapes.Ellipse,
            ksize: new Size(
                makeupParams.linerKernelW,
                makeupParams.linerKernelH));
        
        Cv2.Dilate(
            src: linerMask,
            dst: linerMask,
            element: kernel,
            iterations: 1);

        // Erode doesn't produce any results
        //Cv2.Erode(
        //    src: linerMask,
        //    dst: linerMask,
        //    element: kernel,
        //    iterations: 1);

        Mat notLinerMask = linerMask.Clone();
        Cv2.BitwiseNot(src: linerMask, dst: notLinerMask);

        Mat linearRgb = new Mat(
            imageHeight,
            imageWidth,
            MatType.CV_8UC3,
            new Scalar(0, 0, 0));
        Cv2.CvtColor(
            src: notLinerMask,
            dst: linearRgb,
            code: ColorConversionCodes.GRAY2BGR);

        Mat foreImage = new Mat(
            imageHeight,
            imageWidth,
            MatType.CV_8UC3,
            new Scalar(0, 0, 0));
        Cv2.BitwiseAnd(
            src1: image,
            src2: image,
            dst: foreImage,
            mask: notLinerMask);

        Mat temp = new Mat(
            imageHeight,
            imageWidth,
            MatType.CV_8UC3,
            new Scalar(0, 0, 0));
        Cv2.BitwiseAnd(
            src1: linearRgb,
            src2: linearRgb,
            dst: temp,
            mask: linerMask);
        Cv2.BitwiseOr(
            src1: foreImage,
            src2: temp,
            dst: temp);

        // Blurs the whole frame
        Cv2.GaussianBlur(
            src: temp,
            dst: temp,
            ksize: new Size(9, 9),
            sigmaX: 0);

        Cv2.AddWeighted(
            src1: temp,
            alpha: 0.7,
            src2: image,
            beta: 0.3,
            gamma: 0,
            dst: image);

        return MatToTexture(image);
    }
}
