using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public DialogueTrigger dialogueTrigger;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    public int playerWallDamage = 1;
    [HideInInspector] public bool playersTurn = true;
    [HideInInspector] public bool swordEquipped;
    public int level = 1;
    public AudioMixerGroup pitchBendGroup;

    public int playerMoved;
    public int foodEaten;
    public int wallBroken;
    public int damagedWall;
    public int attacked;
    public int totalAttacked;
    public bool notAttacked = true;
    public int totalMonstersKilled;
    public int monstersKilled;
    public bool askToSkipDialogue;
    public bool narratorDead;

    public int rollEyes;
    public int stopRollEyes;

    public bool dialogueEnabled = true;

    private int dialogueTrigger1 = 4;
    private int dialogueTrigger2 = 7;
    private int dialogueTrigger3 = 10;
    private bool triggerDialogue4 = true;
    private bool dialogueSwitch;
    private bool zEnabled;
    private int zPressed;
    private int doesNotKill;
    private bool zPressedDialogue;
    private bool fPressed;

    private Coroutine helpCoroutine;
    private Dialogue dialogue;

    private bool pacifist;
    private bool axeStolen;
    private int gameEnding;
    private bool cheater;

    private Text enterText;
    private Text levelText;
    private Text foodText;
    private GameObject levelImage;
    private GameObject helpImage;
    private GameObject snowbear;
    [HideInInspector] public List<Enemy> enemies;
    private bool enemiesMoving;
    [HideInInspector] public bool doingSetup = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        notAttacked = true;
        DontDestroyOnLoad(gameObject);
        dialogue = new Dialogue();
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        dialogueTrigger = GetComponent<DialogueTrigger>();
        snowbear = GameObject.Find("Snowbear");
        helpImage = GameObject.Find("HelpImage");
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        enterText = GameObject.Find("EnterText").GetComponent<Text>();
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        SoundManager.instance.musicSource.outputAudioMixerGroup = pitchBendGroup;
        helpImage.SetActive(false);
        levelImage.SetActive(false);
        gameEnding = 0;
        Invoke("ActuallyStart", levelStartDelay);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    static private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        instance.level++;
        instance.InitGame();
    }
    public void ActuallyStart()
    {
        snowbear.SetActive(false);
        levelImage.SetActive(true);
        levelText.text = "Cave Escape";
        StartCoroutine(BlinkText());
        StartCoroutine(Begin());
    }
    private IEnumerator Begin()
    {
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                levelImage.SetActive(false);
                helpImage.SetActive(true);
                Invoke("StartGame", 0.1f);
                break;
            }
            yield return null;
        }
    }
    public void StartGame()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkText());
        StartCoroutine(Restart());
    }
    void InitGame()
    {
        doingSetup = true;
        GameObject.Find("HelpImage").SetActive(false);
        GameObject.Find("Snowbear").SetActive(false);
        levelImage = GameObject.Find("LevelImage");
        dialogueTrigger = GetComponent<DialogueTrigger>();
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        enterText = GameObject.Find("EnterText").GetComponent<Text>();
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        enterText.enabled = false;
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        StartCoroutine(Dialogue());


        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        if (dialogueEnabled)
        {
            Invoke("TriggerDialogue", .5f);
        }
        else
        {
            doingSetup = false;
        }
    }
    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        StartCoroutine(Restart());
        StartCoroutine(BlinkText());
    }
    private IEnumerator RollEyes()
    {
        int i = 0;
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                i++;
                if (i == rollEyes)
                {
                    yield return new WaitForSeconds(.5f);
                    GameObject.Find("Player").GetComponent<Player>().RollEyes();
                    if (i == stopRollEyes)
                    {
                        break;
                    }
                }
            }
            yield return null;
        }
    }
    private void TriggerDialogue()
    {
        if (playerFoodPoints != 0)
        {
            if (wallBroken == 5)
            {
                doingSetup = true;
                dialogue.sentences = new string[] { "Fun fact: you've broken four walls now.", "Or rather you didn't really break them, but just pressed the arrow/WASD keys.", "Hey, now we've both broken the fourth wall. ;)" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                wallBroken++;
            }
            else if (totalAttacked > 30)
            {
                doingSetup = true;
                dialogue.sentences = playerWallDamage == 1
                    ? new string[] { "Why won't you just die already??", "You've been attacked " + totalAttacked + " times!", "What in the world is inside of those soda bottles??" }
                    : new string[] { "Why won't you just die already??", "You've been attacked " + totalAttacked + " times!", "My plan to get my axe back is taking far too long. ;)" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                totalAttacked = -1000;
            }
        }

        switch (level)
        {
            case 1:
                if (playerMoved == 0)
                {
                    if (gameEnding != 0)
                    {
                        rollEyes = 1;
                        stopRollEyes = 3;
                        StartCoroutine(RollEyes());
                        if (askToSkipDialogue)
                        {
                            dialogue.sentences = new string[] { "Hiya!, I'm here to help!", "Hey, I remember that look!", "This is not your first time in this cave, is it?", "Well I don't need to tell you everything again if you've already heard it.", "You can press T to toggle the dialogue on and off.", "But if you turn it off, you'll miss out on great jokes and foolproof pranks!" };
                        }
                        else
                        {
                            dialogue.sentences = new string[] { "Hiya!", "I'm here to help!", "Huh? What's with that look?", "That look of disbelief! I assure you that I will help you in any way I can.", "It could be that you are intimidated by my voice.", "But if you could see me, you would see that there's nothing to worry about.", "Trust me. :)" };
                        }
                        dialogueTrigger.dialogue = dialogue;
                    }
                    dialogueTrigger.TriggerDialogue();
                    helpCoroutine = StartCoroutine(Tutorial());
                }
                else if (playerMoved == dialogueTrigger1 || playerMoved == dialogueTrigger2 || playerMoved == dialogueTrigger3 || foodEaten == 1 || wallBroken == 1 || damagedWall == 1)
                {
                    if (foodEaten == 1)
                    {
                        dialogue.sentences = new string[] { "Oh, did I forget to mention that that's the MONSTERS' food?", "Whoops. ;)", "*quiet cackling*" };
                        dialogueTrigger.dialogue = dialogue;
                        dialogueTrigger.TriggerDialogue();
                        foodEaten++;

                        dialogueTrigger1++;
                        dialogueTrigger2++;
                        dialogueTrigger3++;
                    }
                    else if (wallBroken == 1)
                    {
                        dialogue.sentences = new string[] { "YOU FOOL!", "You've fallen directly into my trap!", "This cave is filled with MONSTERS, who won't be too happy with someone destroying their home.", "Mwahahaha!" };
                        dialogueTrigger.dialogue = dialogue;
                        dialogueTrigger.TriggerDialogue();
                        wallBroken++;

                        dialogueTrigger1++;
                        dialogueTrigger2++;
                        dialogueTrigger3++;
                    }
                    else if (damagedWall == 1)
                    {
                        dialogue.sentences = new string[] { "Look, you've damaged the wall!", "Now just hit it three more times." };
                        dialogueTrigger.dialogue = dialogue;
                        dialogueTrigger.TriggerDialogue();
                        damagedWall++;
                    }
                    else
                    {
                        if (dialogueSwitch && playerMoved == dialogueTrigger2)
                        {
                            dialogue.sentences = foodEaten == 0
                                ? (new string[] { "Soda and tomatoes both give you more food for your journey.", "As the clearly healthier option, you can survive on soda for much longer.", "Check your food count below when you pick up some food to see its nutritional value." })
                                : (new string[] { "There will be monsters in the next room, but don't worry.", "They are completely non-violent as they only eat soda and tomatoes.", "Yes, they eat soda. Didn't know how to get the cap off so...", "Wait!", "I bet they would forgive you for stealing some of their food if you showed them how.", "Well, go on! The monsters are waiting." });
                            dialogueTrigger.dialogue = dialogue;
                            dialogueTrigger.TriggerDialogue();
                            dialogueSwitch = false;
                        }
                        else if (!dialogueSwitch)
                        {
                            if (playerMoved == dialogueTrigger1)
                            {
                                dialogue.sentences = new string[] { "Every time you move you consume 1 piece of food.", "Your food count is shown below.", "If you run out of food, you will die of starvation." };
                                dialogueTrigger.dialogue = dialogue;
                                dialogueTrigger.TriggerDialogue();
                                dialogueSwitch = true;
                            }
                            else if (playerMoved == dialogueTrigger3)
                            {
                                dialogue.sentences = wallBroken == 0
                                    ? (new string[] { "You might not remember this, but you were mining in a floor above when the ground collapsed.", "Luckily, you still have your axe, so if you move into a wall, you'll chop it.", "Try chopping down a wall. The inside walls can be destroyed in four hits." })
                                    : (new string[] { "To be clear, I was just joking around about the walls earlier. ", "The monsters can't get past walls easily, so they'd appreciate someone destroying them.", "I'm sensing the beginning of many new friendships!" });
                                dialogueTrigger.dialogue = dialogue;
                                dialogueTrigger.TriggerDialogue();
                                dialogueSwitch = true;
                            }
                        }
                    }
                }

                break;
            case 2:
                if (dialogueSwitch)
                {
                    int nullElement = 4;
                    if (gameEnding == 0)
                    {
                        dialogue.sentences = new string[] { "Watch out!", "It's a MONSTER!", "Just kidding! These monsters wouldn't hurt a fly.", "The weak skeletal ones are called Gart and the tougher, stronger ones with red jackets are called Geke.", null, "Go say hi to your future best friend!" };
                    }
                    else
                    {
                        rollEyes = 2;
                        stopRollEyes = 2;
                        StartCoroutine(RollEyes());
                        nullElement = 5;
                        dialogue.sentences = new string[] { "Watch out!", "It's a MONSTER!", "Just kidding! These monsters wouldn't hurt a fly.", "There's that look again! And Gart and Geke say they've seen you before.", "You do look oddly familiar, come to think of it. Do I know you?", null, "Go say hi to Gart and Geke. It would be rude not to." };
                    }
                    if (wallBroken > 0)
                    {
                        dialogue.sentences[nullElement] = foodEaten == 0
                            ? "They'll help you escape, since you didn't steal their food."
                            : "Despite you destroying their cave walls aaaand stealing their food, they forgive you.";
                    }
                    else
                    {
                        dialogue.sentences[nullElement] = foodEaten == 0
                            ? "They already really like you for leaving alone their food and walls!"
                            : "They want to give you some food, so you don't have to resort to stealing to survive.";
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueSwitch = false;
                    playerMoved = 0;
                }
                else if (attacked == 1)
                {
                    if (wallBroken > 0)
                    {
                        dialogue.sentences = foodEaten == 0
                            ? new string[] { "YOU FOOL!", "Me, the genius mastermind, has tricked you once more!", "These monsters don't care that you left that junk food on the ground!", "They are hungry for YOU.", "Mwahahaha!" }
                            : new string[] { "YOU FOOL!", "Do you really think the monsters would forgive you THAT easily??", "After you ATE their food and DESTROYED their home??", "No.", "The monsters want revenge.", "They'll take all of YOUR food now.", "You did this to yourself." };
                    }
                    else
                    {
                        dialogue.sentences = foodEaten == 0
                            ? new string[] { "YOU FOOL!", "You, Mr. Goody Two Shoes, actually believed that the monsters cared about the junk and walls down here!?", "Or were you just trying to outsmart me?", "Either way, you're a failure.", "BOOM ROASTED", "The monsters don't even need to attack you anymore, because I just ended your life." }
                            : new string[] { "YOU FOOL!", "You took their food, and now the monsters want it BACK!", "Not just that, they won't stop until they take ALL of your food as payback.", "You could say they are...", "Soda-termined. ;)", "So what if you die of starvation?", "Soda-pressing ;)", "Sounds like a YOU problem." };
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    attacked++;
                }
                else if ((foodEaten == 1 || wallBroken == 1) && attacked == 0)
                {
                    if (foodEaten == 1)
                    {
                        dialogue.sentences = new string[] { "You're stealing their food right in front of them???", "Even after I told you how much they appreciated you NOT taking their food?", "What an absolute savage." };
                        foodEaten++;
                    }
                    else
                    {
                        dialogue.sentences = new string[] { "But why?", "What did that do for you?", "It would've required less energy to just go a different way!", "When you're done being a rebel without a cause, the monster would love to meet you." };
                        wallBroken++;
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                else if (playerMoved == 12 && attacked == 0)
                {
                    dialogue.sentences = new string[] { "Why are you avoiding the monster?", null, "You'd rather have a friend than an enemy right?" };
                    if (wallBroken > 0)
                    {
                        dialogue.sentences[1] = foodEaten == 0
                            ? "You helped them by destroying unnecessary walls, so don't worry about it."
                            : "Everyone deserves a second chance, even you.";
                    }
                    else
                    {
                        dialogue.sentences[1] = foodEaten == 0
                            ? "You've done nothing wrong!"
                            : "You needed to steal the food to survive. They want to help you.";
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    playerMoved++;
                }
                else if (attacked == 3)
                {
                    dialogue.sentences = new string[] { "Yikes.", "You won't last a week down here." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    attacked++;
                }

                break;
            case 3:
                if (!dialogueSwitch)
                {
                    dialogue.sentences = attacked == 0
                        ? new string[] { "Mwahahahaha!", "You probably thought going up to the monster was a trap, right?", "WRONG.", "Going up to monsters is how you ATTAC-", "...", "Going up to monsters is definitely bad.", "Trust me. Have I ever lied to you?" }
                        : new string[] { "Alright, I've had my fun.", "I'll help you out now.", "I'm not a MONSTER. ;)", "You can move twice before the monsters move once.", "Go up to the monster and then press Z and move at the monster to attack.", "You can kill Gart all you want, but Geke would require two hits." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueSwitch = true;
                    if (attacked > 0)
                    {
                        notAttacked = false;
                        zEnabled = true;
                    }
                }
                else if (attacked == 1 && triggerDialogue4)
                {
                    if (notAttacked)
                    {
                        dialogue.sentences = new string[] { "YOU FOOL!", "You should've seen your face when you blundered right into my trap!", "I even told you that going up to monsters was bad!", "Mwahahaha!" };
                        StartCoroutine(Dialogue5());
                        zEnabled = true;
                        notAttacked = false;
                    }
                    else
                    {
                        dialogue.sentences = zPressedDialogue
                            ? new string[] { "Arghh!", "You're pressing Z, but not at the right time!", "Listen carefully.", "If you are adjacent to a monster, press Z and the arrow or WASD key in its direction.", "Try not to fail this time? ;)" }
                            : new string[] { "YOU FOOL!", "You just walked right into the monster's attack!", "You have to be strategic with your moves to move next to the monster without getting hurt.", "Then hold Z and move at the monster.", "Must I explain EVERYTHING to you?" };
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                attacked = 0;
                zPressedDialogue = false;
                break;
            case 4:
                if (dialogueSwitch)
                {
                    if (!zEnabled)
                    {
                        if (foodEaten == 0 && wallBroken == 0)
                        {
                            dialogue.sentences = new string[] { "What are you doing?", "You've just gone straight to the exit every single time.", "You're going to die alone, having lived a boring life.", "OOOOOOOH! That's what you get for ignoring my advice. :(" };
                            StartCoroutine(Dialogue6());
                        }
                        else
                        {
                            dialogue.sentences = new string[] { "Don't think you're some big hotshot for avoiding my trap.", "Only a moron would just walk up to a scary monster ;)", "This is not some pacifist, everybody loves each other world.", "Press Z and the key in the direction of the monster to perform a melee attack and fight back.", "Since you're such a smartypants, I won't help you with setting up the attack.", "Good luck!" };
                            zEnabled = true;
                        }
                    }
                    else if (zPressed == 0)
                    {
                        dialogue.sentences = new string[] { "Truly, I'm delighted to see you aren't attacking the monsters.", "These are some of my most loyal friends...", "...except Gart. Kill Gart.", "Just kidding! :) Thank you, really." };
                        doesNotKill = 1;
                    }
                    else if (zPressed < 3)
                    {
                        dialogue.sentences = new string[] { "I have to give you some credit.", "You only fell for the prank that the Z button allows you to attack " + zPressed + " times.", "I was supposed to just tell you that the Z button allows you to skip a move but...", "What's the fun in that? ;)" };
                        zEnabled = false;
                    }
                    else
                    {
                        dialogue.sentences = new string[] { "Don't feel bad.", "It's very hard to not fall for my ingenius traps.", "You aren't the only fool who has fallen down here.", "But for your sake, I'll stop fool-ing around now. ;)" };
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueSwitch = false;
                }
                else if (attacked == 1 && triggerDialogue4)
                {
                    if (doesNotKill == 1)
                    {
                        dialogue.sentences = zPressedDialogue
                            ? new string[] { "BETRAYAL!", "You just tried to attack my friend! Right after I thanked you too.", "Good luck figuring out what you did wrong...", "NOT!", "Mwahahaha!" }
                            : new string[] { "YOU FOOL!", "You just walked right into the monster's attack!", "You have to be strategic with your movements to avoid their attacks." };
                    }
                    else
                    {
                        dialogue.sentences = zPressedDialogue
                            ? new string[] { "Oh looks like you're not so smart after all.", "You need to press Z and move at the monster at the same time.", "Is that too difficult for you?" }
                            : new string[] { "YOU FOOL!", "You just walked directly into the monster's attack!", "You have to move next to the monster right after he moves.", "Mark your calendars.", "I was wrong about something.", "Turns out you're not smart at all. ;)" };
                    }
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                attacked = 0;
                zPressedDialogue = false;
                break;
            case 14:
                if (playerWallDamage > 1)
                {
                    dialogue.sentences = new string[] { "Great you found my axe!", "Hmm?", "FINDERS KEEPERS?!", "That's MINE. THIEF!", "Ah whatever.", "I'll just take it from your corpse when you die. ;)" };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueSwitch = true;
                }
                else
                {
                    dialogue.sentences = new string[] { "Main characters like quests, right?", "Here's one for you.", "Go find my axe for me. It could chop down walls twice as fast as the rusted trash you have can." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }

                break;
            case 15:
                if (playerWallDamage == 1)
                {
                    dialogue.sentences = new string[] { "Man, if you can't even complete that side quest, you have no chance of escaping." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                else
                {
                    doingSetup = false;
                }

                break;
            case 21:
                if (swordEquipped)
                {
                    dialogue.sentences = new string[] { "...", "So, this is probably where you expect a tutorial box to pop up.", "I would tell you how to use it, maybe make some joke too.", "No.", "I will not help you kill Geke.", "I will not help you kill Gart.", "You're on your own.", "..." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueSwitch = false;
                    StartCoroutine(ManDown());
                }
                else
                {
                    dialogue.sentences = new string[] { "...", "......", "Just don't." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                break;
            case 22:
                if (monstersKilled == 0 && !swordEquipped && gameEnding != 3)
                {
                    dialogue.sentences = new string[] { "I knew I could count on you.", "Your journey is almost over." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                else if (monstersKilled == 0 && swordEquipped)
                {
                    dialogue.sentences = new string[] { "But it's not too late to make the right choice." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                else if (gameEnding > 0 && totalMonstersKilled > 0 && swordEquipped)
                {
                    dialogue.sentences = new string[] { "You have killed " + totalMonstersKilled + " monsters now.", "When will it ever be enough?" };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
                else
                {
                    doingSetup = false;
                }
                break;
            case 28:
                dialogue.sentences = swordEquipped
                ? new string[] { "You won't last a week." }
                : new string[] { "Want to hear a joke?", "Why did the monster ask the ghost out?", "He wanted a ghoul-friend. ;)", "Okay, how about this one: Why did the monster ask the zombie out?", "He wanted a zom-bae.", "(You seriously question your decision to not take the sword)", "So funny you forgot to laugh?", "Zombie like that sometimes. ;)" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                dialogueSwitch = true;
                break;
            case 35:
                dialogue.sentences = swordEquipped
                ? new string[] { "I've underestimated you.", "You're a mean, green, killing machine." }
                : new string[] { "You've made it pretty far in this cave. Cleared lots of rooms.", "Now I got a riddle for you.", "In which room are there no zombies?", "The Living Room ;)" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                dialogueSwitch = false;
                break;
            case 48:
                if (cheater)
                {
                    dialogue.sentences = swordEquipped
                        ? new string[] { "You're killing my friends and you are cheating to try and escape??", "Despicable.", "I won't let you escape." }
                        : new string[] { "Not good enough to do it the hard way, so you are trying to cheat?", "Remember this: cheaters never win." };
                }
                else
                {
                    StartCoroutine(EndGame());
                    dialogue.sentences = new string[] { "You've made it farther than I've ever imagined.", "(You can see the exit, so close yet so far away)", "As your reward for making it this far, I'll give you lots of extra burnt pizza crust.", "You're going to need to it to survive. Mwahahaha!", "This might be the most evil thing I've done yet!" };
                }
                boardScript.enemyBonus = 6;
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                break;
            case 49:
                dialogue.sentences = new string[] { "Let's see you get past THIS!" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                break;
            case 50:
                levelImage.SetActive(true);
                levelText.text = cheater
                    ? "You cheated to escape."
                    : "You escaped.";
                SoundManager.instance.musicSource.Stop();
                StopAllCoroutines();
                CancelInvoke();
                break;
            default:
                doingSetup = false;
                break;
        }
    }
    private IEnumerator EndGame()
    {
        int i = 0;
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                i++;
                if (i == 3)
                {
                    playerFoodPoints += 125;
                    foodText.text = "+125 Food: " + playerFoodPoints;
                    break;
                }
            }
            yield return null;
        }
    }
    private IEnumerator ManDown()
    {
        bool ten = true;
        bool fifteen = true;
        while (true)
        {
            if (dialogueEnabled)
            {
                if (monstersKilled == 1)
                {
                    dialogue.sentences = new string[] { "...", "(You hear someone about to say something, but then sighing instead.)", "(The sigh sounded as if their heart had been ripped apart, never to be put back together again.)" };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    monstersKilled++;
                }
                else if (monstersKilled == 11 && ten)
                {
                    dialogue.sentences = new string[] { "STOP!", "I can't take it anymore!", "I'll enable a special move for you.", "If you press F, you will freeze all the monsters.", "You won't need to kill them now.", "Please." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    ten = false;
                    StartCoroutine(F());
                }
                else if (monstersKilled == 16 && fifteen)
                {
                    dialogue.sentences = new string[] { "Is this a game to you??", "You're playing with my heart!", "These are my friends and closest allies you are killing.", "Just freeze them (F) instead.", "Please, I don't know how much more I can take." };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    fifteen = false;
                }
                else if (monstersKilled == 60)
                {
                    SoundManager.instance.musicSource.Stop();
                    dialogue.sentences = new string[] { "(The whole cave goes quiet knowing what you have done.)", "(The narrator has died of grief.)" };
                    narratorDead = true;
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                    dialogueEnabled = false;
                    break;
                }
            }
            yield return null;
        }
    }
    private IEnumerator ToggleDialogue()
    {
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.T))
            {
                if (!narratorDead)
                {
                    dialogueEnabled = dialogueEnabled != true;
                    if (playerFoodPoints > 0)
                    {
                        HideLevelImage();
                    }
                }
                else
                {
                    dialogue.sentences = new string[] { "(But all you hear is silence.)" };
                    dialogueTrigger.dialogue = dialogue;
                    dialogueTrigger.TriggerDialogue();
                }
            }
            yield return null;
        }
    }
    private IEnumerator F()
    {
        while (!fPressed)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.F))
            {
                playerFoodPoints = 0;
                FindObjectOfType<Player>().CheckIfGameOver();
                fPressed = true;
            }
            yield return null;
        }
    }
    private IEnumerator Z()
    {
        while (true)
        {
            if (!doingSetup && Input.anyKeyDown && Input.GetKeyDown(KeyCode.Z))
            {
                foreach (Enemy enemy in enemies)
                {
                    if (!enemy.skipMove || (!enemy.hasMoved && level>=28 && swordEquipped))
                    {
                        playersTurn = false;
                        StartCoroutine(MoveEnemies());
                        break;
                    }
                    enemy.skipMove = false;
                }
                playerFoodPoints--;
                foodText.text = "Food: " + playerFoodPoints;
                if (zEnabled && dialogueEnabled)
                {
                    zPressed++;
                    zPressedDialogue = true;
                    if (zPressed == 3)
                    {
                        yield return new WaitForSeconds(.5f);
                        if (level == 3)
                        {
                            dialogue.sentences = new string[] { "YOU FOOL!", "Pressing Z actually skips your next move!", "I can't believe you tried it three times!", "Bamboozled again! Mwahahahaha!" };
                        }
                        else if (doesNotKill == 1)
                        {
                            dialogue.sentences = new string[] { "YOU FOOL!", "Z actually skips your next move! You fell for it three times!", "Vengeance is MINE!", "But that's not all!", "The award for best acting goes to me.", "Truthfully, there's no place for killers in this world.", "But if someone just 'accidentally' steals too much food and you starve, that's okay ;)" };
                        }
                        else
                        {
                            dialogue.sentences = new string[] { "YOU FOOL!", "Z skips your next move! After 3 times, you still didn't get that?", "Mr. Smartypants more like Mr. Fartypants, am I right?", "But that's not all!", "The award for best acting goes to me.", "Truthfully, there's no place for killers in this world.", "But if someone just 'accidentally' steals too much food and you starve, that's okay ;)" };
                        }
                        dialogueTrigger.dialogue = dialogue;
                        dialogueTrigger.TriggerDialogue();
                        triggerDialogue4 = false;
                    }
                }
            }
            yield return null;
        }
    }
    private IEnumerator Dialogue5()
    {
        int temp = playerMoved;
        while (true)
        {
            if (playerMoved > temp)
            {
                dialogue.sentences = new string[] { "Now that we've established that I'm way smarter than you, I'll help you out.", "You can move twice before the monsters move once.", "So, go up to the monster and hold Z and move at the monster to attack.", "You can kill Gart all you want, but Geke would require two hits." };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                break;
            }
            yield return null;
        }
    }
    private IEnumerator Dialogue6()
    {
        int temp = playerMoved;
        while (true)
        {
            if (playerMoved > temp)
            {
                dialogue.sentences = new string[] { "By the way, you've missed out on a lot of my tips and tricks...", "Including a special attack move so, HA!" };
                dialogueTrigger.dialogue = dialogue;
                dialogueTrigger.TriggerDialogue();
                break;
            }
            yield return null;
        }
    }
    private IEnumerator Tutorial()
    {
        while (true)
        {
            if (dialogueEnabled
                && ((level == 50) || (level == 4 && attacked == 1 && triggerDialogue4) || (level == 21 && swordEquipped && dialogueSwitch) || (wallBroken == 5) || (level == 14 && playerWallDamage > 1 && !dialogueSwitch) || (level == 3 && dialogueSwitch && (attacked == 1)) || (level == 1 && ((dialogueSwitch && playerMoved == dialogueTrigger2) || (!dialogueSwitch && (playerMoved == dialogueTrigger1 || playerMoved == dialogueTrigger3)) || (foodEaten == 1) || (wallBroken == 1) || (damagedWall == 1))) || (level == 2 && (attacked == 3 || attacked == 1 || (playerMoved == 12 && attacked == 0) || ((foodEaten == 1 || wallBroken == 1) && attacked == 0)))))
            {
                TriggerDialogue();
            }
            yield return null;
        }
    }
    private IEnumerator Dialogue()
    {
        if (dialogueEnabled)
        {
            switch (level)
            {
                case 7:
                    yield return new WaitForSeconds(levelStartDelay);
                    levelText.text = "The monsters are hungry...";
                    break;
                case 14:
                    yield return new WaitForSeconds(levelStartDelay);
                    levelText.text = "Have you seen my axe?";
                    if (axeStolen)
                    {
                        yield return new WaitForSeconds(levelStartDelay);
                        levelText.text = "Of course you have.";
                        yield return new WaitForSeconds(levelStartDelay);
                        levelText.text = "YOU STOLE IT";
                    }

                    break;
                case 21:
                    yield return new WaitForSeconds(levelStartDelay);
                    levelText.text = "Don't you dare...";
                    break;
                case 22:
                    yield return new WaitForSeconds(levelStartDelay);
                    if (swordEquipped)
                    {
                        if (gameEnding <= 0)
                            levelText.text = "Wrong choice.";
                        else if (gameEnding == 3)
                        {
                            levelText.text = "Killing again?";
                            yield return new WaitForSeconds(levelStartDelay);
                            levelText.text = "You monster.";
                        }
                        else if (gameEnding < 3)
                        {
                            levelText.text = "You'll pay for this.";
                            yield return new WaitForSeconds(levelStartDelay);
                            levelText.text = "I thought you were different.";
                        }
                    }
                    else
                    {
                        if (gameEnding == 3)
                        {
                            levelText.text = "You aren't forgiven.";
                            boardScript.enemyBonus = 5;
                        }
                        else
                        {
                            levelText.text = "The monsters thank you.";
                            yield return new WaitForSeconds(levelStartDelay);
                            levelText.text = "This exit escapes.";
                            pacifist = true;
                        }
                    }

                    break;
                case 23:
                    yield return new WaitForSeconds(levelStartDelay);
                    if (!swordEquipped)
                    {
                        if (gameEnding == 3)
                        {
                            levelText.text = "Die.";
                            SoundManager.instance.musicSource.pitch = 1.5f;
                        }
                        else
                        {
                            levelText.text = "Psych!";
                            if (gameEnding > 0)
                            {
                                yield return new WaitForSeconds(levelStartDelay);
                                levelText.text = "Huh?";
                                yield return new WaitForSeconds(levelStartDelay);
                                levelText.text = "You look unsurprised.";
                                yield return new WaitForSeconds(levelStartDelay);
                                levelText.text = "Great pokerface. ;)";
                            }
                        }
                    }

                    break;
                case 24:
                    yield return new WaitForSeconds(levelStartDelay);
                    if (!swordEquipped && gameEnding == 3)
                    {
                        levelText.text = "Mission Failed.";
                        yield return new WaitForSeconds(levelStartDelay);
                        levelText.text = "We'll get 'em next time.";
                    }

                    break;
                case 28:
                    yield return new WaitForSeconds(levelStartDelay);
                    if (swordEquipped)
                    {
                        if (gameEnding <= 0)
                            levelText.text = "You're gonna have a bad time.";
                        else if (gameEnding == 3)
                            levelText.text = "I'm going to enjoy this.";
                        else
                        {
                            levelText.text = "Time to pay.";
                        }
                        SoundManager.instance.musicSource.pitch = 1.5f;
                        //pitchBendGroup.audioMixer.SetFloat("pitchBend", 1f / 1.5f);
                    }

                    break;
                case 49:
                    yield return new WaitForSeconds(levelStartDelay);
                    levelText.text = "This is it.";
                    yield return new WaitForSeconds(levelStartDelay);
                    levelText.text = "Escape or Die.";
                    SoundManager.instance.musicSource.pitch = 1.75f;
                    break;
            }
        }

        Invoke("HideLevelImage", levelStartDelay);
    }
    private IEnumerator Restart()
    {
        /* gameEnding 3 is equipping the sword
         * gameEnding 2 is equipping the axe and made it past level 23
         * gameEnding 1 is neither but made it past level 21
         * gameEnding 0 is first time playing
         * gameEnding -1 is future times playing
         */
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                axeStolen |= playerWallDamage > 1;
                askToSkipDialogue |= (gameEnding > 0 || gameEnding == -1);
                if (cheater)
                {
                    enterText.enabled = false;
                    levelText.text = "Told ya. ;)";
                    yield return new WaitForSeconds(levelStartDelay);
                }
                if (swordEquipped)
                {
                    gameEnding = 3;
                    if (fPressed)
                    {
                        enterText.enabled = false;
                        levelText.text = "Whoops ;)";
                        yield return new WaitForSeconds(levelStartDelay);
                        gameEnding = -1;
                    }
                }
                else if (gameEnding == 3)
                {
                    if (monstersKilled > 0)
                        monstersKilled--;
                    if (level > 22)
                    {
                        enterText.enabled = false;
                        levelText.text = "Now we're even.";
                        yield return new WaitForSeconds(levelStartDelay);
                        gameEnding = -1;
                    }
                }
                else if (level >= 23)
                {
                    gameEnding = 2;
                }
                else if (pacifist)
                {
                    gameEnding = 1;
                }
                else if (level == 1 && playerFoodPoints == 100) //only true for the very first restart 
                {
                    gameEnding = 0;
                }
                else
                {
                    gameEnding = -1;
                }
                playerMoved = 0;
                totalMonstersKilled += monstersKilled;
                foodEaten = 0;
                wallBroken = 0;
                damagedWall = 0;
                attacked = 0;
                totalAttacked = 0;
                notAttacked = true;
                monstersKilled = 0;

                dialogueTrigger1 = 4;
                dialogueTrigger2 = 7;
                dialogueTrigger3 = 10;
                triggerDialogue4 = true;
                dialogueSwitch = false;
                zEnabled = false;
                zPressed = 0;
                doesNotKill = 0;
                zPressedDialogue = false;
                fPressed = false;
                cheater = false;

                //tested:
                boardScript.enemyBonus = 0;
                level = 0;
                playerFoodPoints = 100;
                playerWallDamage = 1;
                swordEquipped = false;
                SoundManager.instance.SwordEquipped();
                if (!SoundManager.instance.musicSource.isPlaying)
                    SoundManager.instance.musicSource.Play();
                StartCoroutine(MoveEnemies());
                StartCoroutine(Z());
                StartCoroutine(ToggleDialogue());

                StartCoroutine(Cheat());

                SceneManager.LoadScene(0);
                break;
            }
            yield return null;
        }
    }
    private IEnumerator Cheat()
    {
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.C))
            {
                while (true)
                {
                    if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.H))
                    {
                        while (true)
                        {
                            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.E))
                            {
                                while (true)
                                {
                                    if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.A))
                                    {
                                        while (true)
                                        {
                                            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.T))
                                            {
                                                level = 47;
                                                SceneManager.LoadScene(0);
                                                cheater = true;
                                                dialogueEnabled = true;
                                                break;
                                            }
                                            yield return null;
                                        }
                                        break;
                                    }
                                    yield return null;
                                }
                                break;
                            }
                            yield return null;
                        }
                        break;
                    }
                    yield return null;
                }
                break;
            }
            yield return null;
        }
    }
    private IEnumerator BlinkText()
    {
        enterText.enabled = true;
        while (true)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Return))
            {
                enterText.enabled = false;
                break;
            }
            switch (enterText.color.a.ToString())
            {
                case "0":
                    enterText.color = new Color(enterText.color.r, enterText.color.g, enterText.color.b, 1);
                    yield return new WaitForSeconds(.75f);
                    break;

                case "1":
                    enterText.color = new Color(enterText.color.r, enterText.color.g, enterText.color.b, 0);
                    yield return new WaitForSeconds(.75f);
                    break;
            }
        }

    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }
    
    public void Save()
    {
        SaveSystem.SavePlayer(this);
    }
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        level = data.level;
        playerFoodPoints = data.food;
        playerWallDamage = data.wallDamage;
        swordEquipped = data.isEquipped;
        SoundManager.instance.SwordEquipped();
        SceneManager.LoadScene(0);
    }
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}