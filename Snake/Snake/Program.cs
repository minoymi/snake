using System.Numerics;
using Raylib_cs;


int WIDTH = 800;
int HEIGHT = 800;
string TITLE = "Snake";
int FPS = 10;
Color BG_COLOR = Color.SKYBLUE;
Color SNAKE_COLOR = Color.YELLOW;
Color FOOD_COLOR = Color.RED;
int SCALE = 20;
int wScale = WIDTH / SCALE;
int hScale = HEIGHT / SCALE;

int initBodyLength = 3;

Raylib.InitWindow(WIDTH, HEIGHT, TITLE);
Raylib.SetTargetFPS(FPS);
Rectangle rec = new Rectangle(0, 0, WIDTH, -HEIGHT); //god this is stupid lol
RenderTexture2D rTexture = Raylib.LoadRenderTexture(WIDTH, HEIGHT);

List<BodySegment> Body = new List<BodySegment>();
int head_x = SCALE / 2 * wScale;
int head_y = SCALE / 2 * hScale;
int dir_x = 1;
int dir_y = 0;
int food_x;
int food_y;

List<BodySegment> unoccupiedSpaces = new List<BodySegment>(wScale * hScale);
for (int w = 0; w < SCALE; w++)
{
    for (int h = 0; h < SCALE; h++)
    {
        BodySegment emptySpace = new()
        {
            x = w * wScale,
            y = h * hScale,
            life = 0
        };
        unoccupiedSpaces.Add(emptySpace);
    }
}

for (int i = 0; i < initBodyLength; i++)
{
    BodySegment segment = new BodySegment
    {
        x = head_x - wScale * (i + 1),
        y = head_y,
        life = initBodyLength - i
    };
    Body.Add(segment);
    segment.life = 0;
    unoccupiedSpaces.Remove(segment);
}
unoccupiedSpaces.Remove(new BodySegment() { x = head_x, y = head_y, life = 0 });


void DrawSnake()
{
    for (int i = Body.Count - 1; i >= 0; i--)
    {
        var segment = Body[i];
        if (segment.life == 0)
        {
            Raylib.DrawRectangle(segment.x, segment.y, wScale, hScale, BG_COLOR);
            unoccupiedSpaces.Add(segment);
            Body.RemoveAt(i);
            continue;
        }
        Raylib.DrawRectangle(segment.x, segment.y, wScale, hScale, SNAKE_COLOR);
        segment.life -= 1;
        Body[i] = segment;
    }
    Raylib.DrawRectangle(head_x, head_y, wScale, hScale, SNAKE_COLOR);
}

void MoveSnake()
{
    int new_x = head_x + dir_x * wScale;
    int new_y = head_y + dir_y * hScale;

    head_x = new_x;
    head_y = new_y;

    if (head_x < 0) { head_x = WIDTH - wScale; }
    else if (head_x >= WIDTH) { head_x = 0; }
    else if (head_y < 0) { head_y = HEIGHT - wScale; }
    else if (head_y >= HEIGHT) { head_y = 0; }




    foreach (var segment in Body)
    {
        if (head_x == segment.x && head_y == segment.y)
        {
            GameOver();
        }
    }

    if (head_x == food_x && head_y == food_y)
    {
        for (int i = 0; i < Body.Count; i++)
        {
            var segment = Body[i];
            segment.life += 1;
            Body[i] = segment;
        }
        Raylib.DrawRectangle(food_x, food_y, wScale, hScale, BG_COLOR);
        SpawnFood();
    }

    BodySegment newSegment = new()
    {
        x = head_x,
        y = head_y,
        life = Body.Count,
    };
    Body.Add(newSegment);

    newSegment.life = 0;
    unoccupiedSpaces.Remove(newSegment);


}


Random rnd = new Random();
void SpawnFood()
{
    if (unoccupiedSpaces.Count == 0)
    {
        Win();
    }
    int f = rnd.Next(0, unoccupiedSpaces.Count);
    food_x = unoccupiedSpaces[f].x;
    food_y = unoccupiedSpaces[f].y;
    Raylib.DrawRectangle(food_x, food_y, wScale, hScale, FOOD_COLOR);
}

void Win()
{
    string message = "You win ! ! !";
    int fontSize = 100;
    int msgWidth = Raylib.MeasureText(message, fontSize);
    Color msgColor = Color.PINK;

    Raylib.BeginTextureMode(rTexture);
    Raylib.ClearBackground(BG_COLOR);
    Raylib.DrawText(message, WIDTH / 2 - msgWidth / 2, HEIGHT / 2 - fontSize / 2, fontSize, msgColor);
    Raylib.EndTextureMode();
}

bool isgamelost = false;
void GameOver()
{
    isgamelost = true;
    string message = "u lost";
    int fontSize = 100;
    int msgWidth = Raylib.MeasureText(message, fontSize);
    Color msgColor = Color.YELLOW;

    Raylib.BeginTextureMode(rTexture);
    Raylib.ClearBackground(BG_COLOR);
    Raylib.DrawText(message, WIDTH / 2 - msgWidth / 2, HEIGHT / 2 - fontSize / 2, fontSize, msgColor);
    Raylib.EndTextureMode();
}



Raylib.BeginTextureMode(rTexture);
Raylib.ClearBackground(BG_COLOR);
SpawnFood();
Raylib.EndTextureMode();

while (!Raylib.WindowShouldClose())
{

    if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT) && dir_x == 0)
    {
        dir_x = -1;
        dir_y = 0;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT) && dir_x == 0)
    {
        dir_x = 1;
        dir_y = 0;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP) && dir_y == 0)
    {
        dir_y = -1;
        dir_x = 0;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN) && dir_y == 0)
    {
        dir_y = 1;
        dir_x = 0;

    }
    if (!isgamelost)
    {
        Raylib.BeginTextureMode(rTexture);
        DrawSnake();
        MoveSnake();
        Raylib.EndTextureMode();
    }


    Raylib.BeginDrawing();
    Raylib.DrawTextureRec(rTexture.texture, rec, new Vector2(0, 0), Color.WHITE);
    Raylib.EndDrawing();

}


Raylib.CloseWindow();


struct BodySegment
{
    public int x;
    public int y;
    public int life;
}
