using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleTester : MonoBehaviour
{
    System.Random dice = new System.Random();    
    
    public Battle battle;

    public SpeciesCollection species;
    public ElementCollection elements;

    public PartyEditor playerParty;
    public PartyEditor opponentParty;

    public Button stepRoundButton;

    public RectTransform[] winLossTieBars;
    public TextMeshProUGUI[] winLossTieRates;
    public TextMeshProUGUI statsDisplay;

    float _threadStartTime;

    float[] _ratios = new float[3];

    private void Update() {
        if (!_runThread) return;

        float duration = Time.time - _threadStartTime;
        if (duration <= 0f) return;

        long total = _winLossTies[0] + _winLossTies[1] + _winLossTies[2];



        float accumulated = 0f;
        for (int i = 0; i < 3; i++) {
            _ratios[i] = _winLossTies[i] / (float)total;
        }

        // Put ties in the middle for display.
        float swap = _ratios[2];
        _ratios[2] = _ratios[1];
        _ratios[1] = swap;

        for (int i = 0; i < 3; i++) {
            winLossTieRates[i].text = (_ratios[i]).ToString("P");

            winLossTieBars[i].anchorMin = new Vector2(accumulated, 0f);
            accumulated += _ratios[i];
            winLossTieBars[i].anchorMax = new Vector2(accumulated, 1f);
        }        

        string stats = $"{total} matches simulated ({total/duration}/sec)\n" +
            $"{_totalRounds} rounds ({_totalRounds/duration}/sec)\n" +
            $"Average match lasts {_totalRounds/(float)total} rounds";

        statsDisplay.text = stats;       
    }

    long[] _winLossTies = new long[3];
    long _totalRounds;

    public void OnPartyChange(PartyEditor source) {       

        bool ready = true;

        ready &= battle.player.ReadyToBattle();
        ready &= battle.opponent.ReadyToBattle();

        stepRoundButton.interactable = ready;

        if (ready) {
            UpdatePartyDisplay();

            _newPartyDataReady = true;
            _threadStartTime = Time.time;

            if (!_runThread)
                StartThread();
        }
    }

    void UpdatePartyDisplay() {
        playerParty.UpdateHealth(battle.player);
        opponentParty.UpdateHealth(battle.opponent);
    }

    public void StartNewBattle() {
        battle.Start();
    }

    private void OnEnable() {
        playerParty.LinkEditableList(battle.player.party);
        opponentParty.LinkEditableList(battle.opponent.party);

        if (playerParty.list.Widgets.Count == 0)
            playerParty.Add();

        if (opponentParty.list.Widgets.Count == 0)
            opponentParty.Add();

        OnPartyChange(null);
    }

    public void StepRound() {
        if (battle.outcome == Battle.Outcome.PartyDefeated)
            StartNewBattle();

        var winner = battle.EvaluateRound(dice);

        if (winner != Battle.Team.None) {
        }

        UpdatePartyDisplay();
    }

    public void StepMatch() {
        if (battle.outcome == Battle.Outcome.PartyDefeated)
            StartNewBattle();

        var winner = battle.EvaluateMatch(dice);

        _totalRounds += battle.roundCount;

        if (winner != Battle.Team.None) {        
        } else {
            // TODO: Show error message about match running too long.
        }

        UpdatePartyDisplay();
    }


    void StartThread() {
        if (_simThread != null) {
            StopThread();
        }

        _runThread = true;
        _newPartyDataReady = true;
        _simThread = new System.Threading.Thread(BattleLoop);
        _simThread.Start();
    }

    void StopThread() {
        if (_simThread != null) {
            _simThread.Join();
        }
        _simThread = null;
    }

    private void OnDisable() {
        _runThread = false;

        StopThread();
    }

    volatile bool _newPartyDataReady;
    volatile bool _runThread;

    System.Threading.Thread _simThread;

    void BattleLoop() {
        var battle = new Battle();
        var dice = new System.Random();

        while (_runThread) {
            _totalRounds = 0;
            System.Array.Clear(_winLossTies, 0, 3);

            battle.player.party.Clear();
            battle.player.party.AddRange(this.battle.player.party);

            battle.opponent.party.Clear();
            battle.opponent.party.AddRange(this.battle.opponent.party);

            _newPartyDataReady = false;

            if (battle.Start()) {
                while (_runThread && (_newPartyDataReady == false)) {
                    var winner = battle.EvaluateMatch(dice);

                    if (winner != Battle.Team.None) {
                        _winLossTies[(int)winner]++;
                    } else {
                        // TODO: Error message about too-long match.
                    }

                    _totalRounds += battle.roundCount;
                    battle.Start();

                    if (_totalRounds > 999999L) _runThread = false;
                }
            } else {
                _runThread = false;
            }
        }
    }
}
