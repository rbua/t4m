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

function translate_selected(dbClickEvent) {
	var selectedText = window.getSelection().toString().trim();

	if (selectedText !== "") {
		
		var popup_url = chrome.runtime.getURL('popup_div.html');

		fetch(popup_url)
			.then(response => response.text())
			.then(data => {
			  console.log(data);


			  const popupWindow = document.createElement('div');
			  popupWindow.innerHTML = data;

			  console.log(popupWindow.querySelectorAll('h4#selected-text-header'));

			  const selectedTextPopupParagraph = popupWindow.querySelectorAll('h4#selected-text-header')
			  selectedTextPopupParagraph.textContent = selectedText;



			  document.body.appendChild(popupWindow);
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
