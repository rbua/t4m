function PlayAudio(audioData)
{
	const audioCtx = new AudioContext();
	
	audioCtx.decodeAudioData(new Uint8Array(audioData).buffer).then(audioBuffer => {
	  const audioSource = audioCtx.createBufferSource();
	  audioSource.buffer = audioBuffer;
	  audioSource.connect(audioCtx.destination);
	  audioSource.start();
	});
}

function translate_input_on_page(inputText) {
	const url = "https://localhost:7177/Translate/ru/en/" + encodeURIComponent(inputText);
	const options = {
	  method: "POST",
	  headers: { "Content-Type": "application/json" }
	};
  
	fetch(url, options)
	  .then(response => response.json())
	  .then(data => {
		document.getElementById("translated_text_entry").innerHTML = data.translation.translation;
		
		return 
	});
  }

function close_popup_window() {
    const popupWindow = document.querySelector('#popup_translator_window');
    popupWindow.remove();
}


function try_remove_old_popups()
{
	const popupWindow = document.querySelector('#popup_translator_window');
	if (popupWindow) {
	  while (popupWindow.firstChild) {
		popupWindow.removeChild(popupWindow.firstChild);
	  }
	  popupWindow.remove();
	}
}

function calculatePosition(event, elementWidth, elementHeight) {
	const viewportWidth = window.innerWidth;
	const viewportHeight = window.innerHeight;
  
	let top = event.clientY + window.pageYOffset;
	if (top + elementHeight > viewportHeight) {
	  top = viewportHeight - elementHeight;
	}
  
	let left = event.clientX + window.pageXOffset;
	if (left + elementWidth > viewportWidth) {
	  left = viewportWidth - elementWidth;
	}
  
	return { top, left };
  }  

function translate_selected(dbClickEvent) {
	try_remove_old_popups();

	var selectedText = window.getSelection().toString().trim();

	if (selectedText !== "") {
		
		var popup_url = chrome.runtime.getURL('popup_div.html');

		fetch(popup_url)
			.then(response => response.text())
			.then(data => {

			  const popupWindow = document.createElement('div');
			  popupWindow.innerHTML = data;

			  const selectedTextPopupParagraph = popupWindow.querySelectorAll('h4#selected-text-header')
			  selectedTextPopupParagraph.textContent = selectedText;

			  const { top, left } = calculatePosition(dbClickEvent, popupWindow.offsetWidth, popupWindow.offsetHeight);
			  popupWindow.style.position = 'absolute';
			  popupWindow.style.top = `${top}px`;
			  popupWindow.style.left = `${left}px`;

			  document.body.appendChild(popupWindow);
			  
			  const closeButton = document.querySelector('#popup-close-button');
			  if (closeButton) {
				closeButton.addEventListener('click', close_popup_window);
			  }
			  
			})
			.catch(error => console.error(error));
	}
}

function link_popup_css_to_html()
{
	const link = document.createElement("link");
	link.href = chrome.runtime.getURL("popup_styles.css");
	link.type = "text/css";
	link.rel = "stylesheet";
	document.head.appendChild(link);
}

link_popup_css_to_html();
document.addEventListener("dblclick", function(e) { translate_selected(e) });
