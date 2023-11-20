const { ccclass, property } = cc._decorator;

kit.Event.GAME_SEQUENCE_START = 'GAME_SEQUENCE_START';
kit.Event.GAME_SEQUENCE_CORE = 'GAME_SEQUENCE_CORE';
kit.Event.GAME_SEQUENCE_RESULT = 'GAME_SEQUENCE_RESULT';
kit.Event.GAME_SEQUENCE_RESULT_LOSE = 'GAME_SEQUENCE_RESULT_LOSE';
kit.Event.GAME_SEQUENCE_FAKE_LEVEL = 'GAME_SEQUENCE_FAKE_LEVEL';

@ccclass
export default class GameSequence extends cc.Component {
	protected isFakeOnLastStage = false;

	onLoad() {
		this._init();
	}
	onEnable(): void {
		this._subscribeToEvents(true);
	}
	onDisable(): void {
		this._subscribeToEvents(false);
	}

	protected start(): void {
		const targetFps =
			window.spVars && window.spVars.target_fps && window.spVars.target_fps.value
				? window.spVars.target_fps.value
				: 60;
	}

	private _subscribeToEvents(isOn: boolean): void {
		const func = isOn ? 'on' : 'off';

		cc.systemEvent[func](kit.Event.GAME_SEQUENCE_START, this.onGameSequenceStart, this);
		cc.systemEvent[func](kit.Event.GAME_SEQUENCE_CORE, this.onGameSequenceCore, this);
		cc.systemEvent[func](kit.Event.GAME_SEQUENCE_RESULT, this.onGameSequenceResult, this);
		cc.systemEvent[func](kit.Event.GAME_SEQUENCE_RESULT_LOSE, this.onGameSequenceResultLose, this);
		cc.systemEvent[func](kit.Event.FIND_PANEL_ALL_CHECKED, this.onFindPanelAllChecked, this);
		cc.systemEvent[func](kit.Event.GAME_SEQUENCE_FAKE_LEVEL, this.onFindPanelFakeLevel, this);
		cc.systemEvent[func](kit.Event.ITEMS_SHOW_LAST_STAGE, this.onItemsShowLastStage, this);
	}

	private _init(): void {
		const isFakeOnLastStage = kit.dictionary.getItemValue('IsFakeOnLastStage');
		if (isFakeOnLastStage) this.isFakeOnLastStage = isFakeOnLastStage;
	}
	private _enableStart(): void {}
	private _enableCore(): void {
		cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_ButtonsScale', true);
	}
	private _enableResult(): void {
		cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_TutorialHand', false);
		cc.systemEvent.emit(kit.Event.ITEMS_FINISH);
		cc.systemEvent.emit(kit.Event.SOUND_PLAY, 'win', 'play');

		this.scheduleOnce(() => {
			cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_ResultButton', true);
			cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_ResultText', true);
		}, 1);
	}
	private _enableResultLose(): void {
		cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_TutorialHand', false);
		cc.systemEvent.emit(kit.Event.ITEMS_FINISH, false);
		cc.systemEvent.emit(kit.Event.SOUND_PLAY, 'lose', 'play');
		cc.systemEvent.emit(kit.Event.ANALYTICS_SET_STAGES_COUNT, 1);
		cc.systemEvent.emit(kit.Event.ANALYTICS_NEXT_STAGE);

		this.scheduleOnce(() => {
			cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_ResultButtonLose', true);
			cc.systemEvent.emit(kit.Event.UI_ELEMENT_TOGGLE, 'UI_ResultTextLose', true);
		}, 1);
	}

	protected onGameSequenceStart(): void {
		this._enableStart();
	}
	protected onGameSequenceCore(): void {
		this._enableCore();
	}
	protected onGameSequenceResult(): void {
		this._enableResult();
	}
	protected onGameSequenceResultLose(): void {
		this._enableResultLose();
	}
	protected onFindPanelAllChecked(key) {
		if (key !== 'UI_HP') return;

		cc.systemEvent.emit(kit.Event.START_STAGE, 'ResultLose');
	}
	protected onFindPanelFakeLevel(): void {}
	protected onItemsShowLastStage(): void {
		if (!this.isFakeOnLastStage) return;

		cc.systemEvent.emit(kit.Event.START_STAGE, 'FakeLevel');
		cc.systemEvent.emit(kit.Event.ANALYTICS_SET_STAGES_COUNT, 1);
		cc.systemEvent.emit(kit.Event.ANALYTICS_NEXT_STAGE);
	}
}
