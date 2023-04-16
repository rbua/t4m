 /* ---------- ANIMATIONS ----------- */

let letters = 'абвгґдђеёєжзѕиіїйјклљмнњоөпрстћуўфхцчџшщъыьэюяabcdefghijklmnopqrstuvwxyzßäöüàáâäæçèéêëìíîïñóöøúü';
let translation_text_animation_time_id = 0;

 function replaceRandomLetter(text) {
	const randomIndex = Math.floor(Math.random() * text.length);
	const randomLetter = letters.charAt(Math.floor(Math.random() * letters.length));
	return text.substring(0, randomIndex) + randomLetter + text.substring(randomIndex + 1);
}

 function animate_translation_sample_text()
 {
	 const originalText = document.getElementById('selected-text-header').textContent;
	 const words = originalText.split(' ');
	 const wordLengths = words.map(word => word.length);
 
	 const randomTextContainer = document.getElementById('selected-text-translation-header');
	 const randomText = generateRandomText(wordLengths);
	 randomTextContainer.textContent = randomText;
	
	 translation_text_animation_time_id = setInterval(() => {
		randomTextContainer.textContent = replaceRandomLetter(randomText);
	  }, 50);
	  
	  function generateRandomText(wordLengths) {
		return wordLengths.map(length => {
		  let word = '';
		  for (let i = 0; i < length; i++) {
			word += letters.charAt(Math.floor(Math.random() * letters.length));
		  }
		  return word;
		}).join(' ');
	  }
}

/* ---------- ANIMATIONS ----------- */
 



const LanguageList = Object.freeze({
	DE: 'de',
	EN: 'en',
	RU: 'ru',
	UA: 'uk'
  });

function add_options_to_select(select) {
	for (const key in LanguageList) {
	  const option = document.createElement('option');
	  option.value = LanguageList[key];
	  option.textContent = key;

	  select.appendChild(option);
	}
}

function populate_select_with_options() {
	  add_options_to_select(document.getElementById('fromLanguageSelect'));
	  add_options_to_select(document.getElementById('toLanguageSelect'));
}

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

function translate_input_on_page(inputText, fromLanguage, toLanguate) {
	const url = `https://localhost:7177/Translate/${fromLanguage}/${toLanguate}/${encodeURIComponent(inputText)}`;
	const options = {
	  method: "POST",
	  headers: { "Content-Type": "application/json" }
	};
  
	fetch(url, options)
	  .then(response => response.json())
	  .then(data => {
		clearInterval(translation_text_animation_time_id);
		document.getElementById("selected-text-translation-header").innerHTML = data.translation.translation;
		document.getElementById("selected-text-translation-header").style.color = 'black';

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

function add_change_language_handlers()
{
	var selectedText = window.getSelection().toString().trim();

	const fromLanguageSelect = document.getElementById("fromLanguageSelect");
	const toLanguageSelect = document.getElementById("toLanguageSelect");
	
	const fromLanguageFlag = document.getElementById("fromLanguageFlag");
	const toLanguageFlag = document.getElementById("toLanguageFlag");

	const fromImgPath = `https://raw.githubusercontent.com/rbua/t4m/master/Frontend/static_resources/language_icons/${fromLanguageSelect.value}.png`;
	fromLanguageFlag.setAttribute("src", fromImgPath);

	const toImgPath = `https://raw.githubusercontent.com/rbua/t4m/master/Frontend/static_resources/language_icons/${toLanguageSelect.value}.png`;
	toLanguageFlag.setAttribute("src", toImgPath);

	fromLanguageSelect.addEventListener("change", function () {
		const imgPath = `https://raw.githubusercontent.com/rbua/t4m/master/Frontend/static_resources/language_icons/${this.value}.png`;
		fromLanguageFlag.setAttribute("src", imgPath);
		translate_input_on_page(selectedText, fromLanguageSelect.value, toLanguageSelect.value);
	});

	toLanguageSelect.addEventListener("change", function () {
		const imgPath = `https://raw.githubusercontent.com/rbua/t4m/master/Frontend/static_resources/language_icons/${this.value}.png`;
		toLanguageFlag.setAttribute("src", imgPath);
		translate_input_on_page(selectedText, fromLanguageSelect.value, toLanguageSelect.value);
	});
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

			  translate_input_on_page(selectedText, 'auto', 'ru');

			  document.body.appendChild(popupWindow);
			  set_popup_window_position(dbClickEvent, popupWindow);
			  add_close_popup_window_on_button_click_handler();
			  populate_select_with_options();
			  add_change_language_handlers();
			  animate_translation_sample_text();
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
add_close_popup_window_on_page_click_handler();