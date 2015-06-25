﻿/* 
QuickScroll
Copyright 2015 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using UnityEngine;

namespace QuickScroll {

	public partial class QScroll : MonoBehaviour {
		private static bool isSimple {
			get {
				return EditorLogic.Mode == EditorLogic.EditorModes.SIMPLE;
			}
		}

		private static bool isOverArrow {
			get {
				Rect _rect = new Rect (0, 0, 60, 24);
				return _rect.Contains(Mouse.screenPos);
			}
		}

		private static bool isOverParts {
			get {
				Vector3 _position = UIManager.instance.uiCameras [0].camera.WorldToScreenPoint (EditorPartList.Instance.transformTopLeft.position);
				Rect _rect = new Rect ();
				_rect.x = _position.x;
				_rect.y = EditorPartList.Instance.footerTransform.position.x;
				_rect.width = EditorPanels.Instance.partsPanelWidth - 78;
				// I like to scroll on the subassemblyHeight ;)
				//_rect.height = Screen.height - _rect.y - EditorPartList.Instance.footerHeight - EditorPartList.Instance.subassemblyHeight - EditorPartList.Instance.subassemblySeperation;
				_rect.height = Screen.height - _rect.y - EditorPartList.Instance.footerHeight;
				return _rect.Contains(Mouse.screenPos);
			}
		}

		private static bool isOverCategories {
			get {
				Rect _rect = new Rect (0, 25, PartCategorizer.Instance.scrollListSub.scrollList.viewableArea.x, PartCategorizer.Instance.scrollListSub.scrollList.viewableArea.y);
				if (!isSimple) {
					_rect.x = _rect.x + PartCategorizer.Instance.scrollListMain.scrollList.viewableArea.x;
				}
				return _rect.Contains(Mouse.screenPos);
			}
		}

		private static bool isOverFilters {
			get {
				if (isSimple) {
					return false;
				}
				Rect _rect = new Rect (0, 25, PartCategorizer.Instance.scrollListMain.scrollList.viewableArea.x, PartCategorizer.Instance.scrollListMain.scrollList.viewableArea.y);
				return _rect.Contains(Mouse.screenPos);
			}
		}

		internal static void Update() {
			if (EditorLogic.fetch.editorScreen != EditorScreen.Parts || !QSettings.Instance.EnableWheelScroll || !EditorPanels.Instance.IsMouseOver ()) {
				return;
			}
			float _scroll = Input.GetAxis ("Mouse ScrollWheel");
			if (_scroll == 0) {
				return;
			}
			if (isOverArrow) {
				if (isSimple) {
					PartCategorizer.Instance.SetAdvancedMode ();
				} else {
					PartCategorizer.Instance.SetSimpleMode ();
				}
				return;
			}
			bool _ModKeyFilterWheel = false;
			bool _ModKeyCategoryWheel = false;
			#if SHORTCUT
			if (QSettings.Instance.EnableWheelShortCut) {
				_ModKeyFilterWheel = Input.GetKey (QSettings.Instance.ModKeyFilterWheel);
				_ModKeyCategoryWheel = Input.GetKey (QSettings.Instance.ModKeyCategoryWheel);
			}
			#endif
			bool _ModKeyWheel = _ModKeyFilterWheel || _ModKeyCategoryWheel;
			if (isOverFilters || (_ModKeyWheel && isOverCategories) || (_ModKeyFilterWheel && isOverParts)) {
				if (isSimple) {
					PartCategorizer.Instance.SetAdvancedMode ();
				}
				QCategory.SelectPartFilter (_scroll > 0);
			} else if (isOverCategories || (_ModKeyCategoryWheel && isOverParts)) {
				QCategory.SelectPartCategory (_scroll > 0);
			} else if (isOverParts) {
				QCategory.SelectPartPage (_scroll > 0);
			}
		}
	}
}