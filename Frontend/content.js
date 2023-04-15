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
		document.getElementById("selected-text-translation-header").innerHTML = data.translation.translation;
		
		return; 
	});
  }

function close_popup_window() {
    const popupWindow = document.querySelector('#popup_translator_window');
	if(!popupWindow)
		return;

    popupWindow.remove();
}

function add_close_popup_window_on_button_click_handler() {
	const closeButton = document.querySelector('#popup-close-button');
	if(!closeButton)
		return;

	if (closeButton) {
	  closeButton.addEventListener('click', close_popup_window);
	}
}

function handle_document_click(event)
{
	const selectedPopupWindow = document.querySelector('#popup_translator_window');

	if(!selectedPopupWindow)
		return;

	if (!selectedPopupWindow.contains(event.target)) {
		close_popup_window();
	}
}

function add_close_popup_window_on_page_click_handler() {
	document.addEventListener('click', handle_document_click); 
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

function adjustToScreenBounds(top, left)
{
	if(top < window.pageYOffset + 150)
	{
		top = window.pageYOffset + 150;
	}
	if(top > (window.pageYOffset +  window.innerHeight - 150))
	{
		top = window.pageYOffset + window.innerHeight - 150;
	}

	if(left < window.pageXOffset + 150)
	{
		left = window.pageXOffset + 150;
	}
	if(left > (window.pageXOffset +  window.innerWidth - 150))
	{
		left = window.pageXOffset + window.innerWidth - 150;
	}

	return { top, left };
}

function calculatePosition(event, elementWidth, elementHeight) {
	const viewportWidth = window.innerWidth;
	const viewportHeight = window.innerHeight;
  
	let top = event.clientY + window.pageYOffset;
	let left = event.clientX + window.pageXOffset;

	return adjustToScreenBounds(top, left);
}  

function set_popup_window_position(dbClickEvent, popupWindow)
{
	const { top, left } = calculatePosition(dbClickEvent, popupWindow.offsetWidth, popupWindow.offsetHeight);
	popupWindow.style.position = 'absolute';
	popupWindow.style.top = `${top}px`;
	popupWindow.style.left = `${left}px`;
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
			  popupWindow.innerHTML = data.replace('Selected Text', selectedText);

			  translate_input_on_page(selectedText);

			  document.body.appendChild(popupWindow);
			  set_popup_window_position(dbClickEvent, popupWindow);
			  add_close_popup_window_on_button_click_handler();
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

function add_chenge_language_handlers()
{
	const fromLanguageSelect = document.getElementById("fromLanguageSelect");
	const toLanguageSelect = document.getElementById("toLanguageSelect");
	
	const fromLanguageFlag = document.getElementById("fromLanguageFlag");
	const toLanguageFlag = document.getElementById("toLanguageFlag");

	fromLanguageSelect.addEventListener("change", function () {
		var imgPath = chrome.runtime.getURL(`static_resources/language_icons/${this.value}.png`);
		fromLanguageFlag.setAttribute("src", `url(${imgPath})`);
	});

	toLanguageSelect.addEventListener("change", function () {
		var imgPath = chrome.runtime.getURL(`static_resources/language_icons/${this.value}.png`);
		toLanguageFlag.setAttribute("src", `url(${imgPath})`);
	});
}

link_popup_css_to_html();
document.addEventListener("dblclick", function(e) { translate_selected(e) });
add_close_popup_window_on_page_click_handler();
add_chenge_language_handlers();