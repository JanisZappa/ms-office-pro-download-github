using UnityEngine;


public class MobileInput : MonoBehaviour
{
    public RectTransform circleLeft, circleRight;
    
    public float circleDiameterCentimeter, circleScreenEdgeDistanceCentimeter;
    
    private static float PixelPerMilimeter { get { return Screen.dpi * .048f; } }
    
    private bool leftControll, rightControll;
    
    public static Vector2 steer, turn;


    private void Start()
    {
        if(!Application.isEditor && !Application.isMobilePlatform)
            Destroy(gameObject);
    }


    private void Update()
    {
        float edgeDist     = circleScreenEdgeDistanceCentimeter * 10 * PixelPerMilimeter;
        float circleRadius = circleDiameterCentimeter * .5f * 10 * PixelPerMilimeter;
        
        Vector2 leftCirclePos  = new Vector2(Screen.width - edgeDist - circleRadius, edgeDist + circleRadius);
        Vector2 rightCirclePos = new Vector2(edgeDist + circleRadius, edgeDist + circleRadius);
        
        circleLeft.anchoredPosition  = leftCirclePos;
        circleRight.anchoredPosition = rightCirclePos;
        
        circleLeft.localScale  = Vector3.one * circleRadius * .02f;
        circleRight.localScale = Vector3.one * circleRadius * .02f;
        
        
        float threshholdSqr = Mathf.Pow(circleRadius * 2, 2);
        
        leftControll  = false;
        rightControll = false;

        Vector2 steerDir = Vector2.zero, turnDir = Vector2.zero;
        if (!Application.isMobilePlatform && Input.GetMouseButton(0))
        {
            Vector2 touchPos = Input.mousePosition;
            
            turnDir  = touchPos - leftCirclePos;
            steerDir = touchPos - rightCirclePos;
            
            leftControll  = turnDir.sqrMagnitude <= threshholdSqr;
            rightControll = steerDir.sqrMagnitude <= threshholdSqr;
        }
        else if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            float bestSteer = 100000, bestTurn = 100000;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector2 touchPos = Input.touches[i].position;
            
                Vector2 tDir    = touchPos - leftCirclePos;
                float   leftSqr = tDir.sqrMagnitude;
                if (leftSqr <= threshholdSqr && leftSqr < bestTurn)
                {
                    turnDir      = tDir;
                    bestTurn     = leftSqr;
                    leftControll = true;
                }
                
                Vector2 sDir     = touchPos - rightCirclePos;
                float   rightSqr = sDir.sqrMagnitude;
                if (rightSqr <= threshholdSqr && rightSqr < bestSteer)
                {
                    steerDir = sDir;
                    bestSteer = rightSqr;
                    rightControll = true;
                }
            }
        }
        
        if (leftControll)
            turnDir = turnDir.normalized;
            
        if (rightControll)
            steerDir = steerDir.normalized;
        
          
        turn  =  leftControll? turnDir : Vector2.zero;
        steer = rightControll? steerDir: Vector2.zero;
    }
}
