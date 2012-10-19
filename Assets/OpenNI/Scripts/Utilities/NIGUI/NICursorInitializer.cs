/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
using UnityEngine;
using System.Collections;

/// @brief An initializer to make sure the cursor on the object it is attached to (a prefab)
/// will register itself on the NIGUI.
/// @ingroup OpenNIGUIUtiliites
[RequireComponent(typeof(NIGUICursor))]
public class NICursorInitializer : MonoBehaviour {

	/// Just an initialization to get the cursor from the prefab and set it to NIGUI
	void Awake () 
    {
        NIGUICursor cursor=gameObject.GetComponent(typeof(NIGUICursor)) as NIGUICursor;
        NIGUI.SetCursor(cursor);
        NIGUI.ResetGroups(); // to make sure we don't have any baggages...
	}
}
