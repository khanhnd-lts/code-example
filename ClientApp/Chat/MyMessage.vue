<template>
  <div class="myself-message-body">
    <div class="message-content">
      <template v-if="message.photo && message.photo != ''">
        <div v-if="message.loading" class="message-image">
          <img
            class="message-image-display img-overlay"
            :src="message.photo"
            alt=""
            @click="onImageClicked(message)"
          />
          <div class="img-loading"></div>
        </div>
        <div v-else class="message-image">
          <img
            @click="onImageClicked(message)"
            class="message-image-display"
            :src="HOST + '/api/fileupload/download/' + message.photo"
            alt=""
          />
        </div>
        <v-tooltip right v-if="message.content && message.content != ''">
          <template v-slot:activator="{ on, attrs }">
            <div
              class="message-text"
              v-bind="attrs"
              v-on="on"
              :style="{
                background: colors.message.myself.bg,
                color: colors.message.myself.text,
              }"
            >
              <!-- <p class="message-username">{{myself.name}}</p> -->
              <p>{{ message.content }}</p>
            </div>
          </template>
          <span>{{
            message.createdTime | moment(timestampConfig.format)
          }}</span>
        </v-tooltip>
      </template>
      <template v-else>
        <v-tooltip right>
          <template v-slot:activator="{ on, attrs }">
            <div
              class="message-text"
              v-bind="attrs"
              v-on="on"
              :style="{
                background: colors.message.myself.bg,
                color: colors.message.myself.text,
              }"
            >
              <!-- <p class="message-username">{{myself.name}}</p> -->
              <p>{{ message.content }}</p>
            </div>
          </template>
          <span>{{
            message.createdTime | moment(timestampConfig.format)
          }}</span>
        </v-tooltip>
      </template>
      <!--<div class="message-timestamp" :style="{'justify-content': 'flex-end'}">
                <template v-if="timestampConfig.relative">
                    {{message.createdTime.toRelative()}}
                </template>
                <template v-else>
                    {{message.createdTime | moment(timestampConfig.format)}}
                </template>
                <v-icon v-if="asyncMode && !message.loading && !message.viewed" :size="14" class="icon-sent">done</v-icon>
                <v-icon v-else-if="asyncMode && !message.loading && message.viewed" :size="14" class="icon-sent viewed">done_all</v-icon>
                <div v-else-if="asyncMode" class="message-loading"></div>
            </div>-->
    </div>
    <!-- <div v-if="profilePictureConfig.myself" class="thum-container">
            <img class="participant-thumb" :src="HOST + '/api/fileupload/download/' + myself.profilePicture"
                 :style="{'width': profilePictureConfig.styles.width, 'height': profilePictureConfig.styles.height, 'border-radius': profilePictureConfig.styles.borderRadius}">
        </div> -->
  </div>
</template>

<script lang="ts">
import { mapGetters, mapMutations } from "vuex";
import { Vue } from "vue-property-decorator";
export default Vue.extend({
  components: {},
  props: {
    message: {
      type: Object,
      required: true,
    },
    asyncMode: {
      type: Boolean,
      required: false,
      default: false,
    },
    colors: {
      type: Object,
      required: true,
    },
    /* onImageClicked: {
                type: Function,
                required: false,
                default: null
            }, */
    profilePictureConfig: {
      type: Object,
      required: true,
    },
    timestampConfig: {
      type: Object,
      required: true,
    },
    myself: {
      type: Object,
      required: false,
      default: {},
    },
  },
  data() {
    return {
      HOST: process.env.VUE_APP_ROOT_API,
    };
  },
  computed: {},
  methods: {
    onImageClicked(message: any) {
      this.$emit("onImageClicked", message);
    },
  },
});
</script>

<style>
.container-message-display .myself-message-body {
  display: flex;
  align-items: flex-end;
  padding-right: 4px;
  justify-content: flex-end;
  min-height: 37px;
}

.container-message-display .myself-message-body .message-content {
  display: flex;
  align-items: flex-end;
  justify-content: flex-start;
  flex-direction: column;
  max-width: 70%;
}

.container-message-display .myself-message-body .participant-thumb {
  width: 25px;
  height: 25px;
  border-radius: 50%;
  margin-left: 10px;
}

.container-message-display .myself-message-body .message-timestamp {
  padding: 2px 7px;
  border-radius: 15px;
  margin: 0;
  /* max-width: 50%; */
  overflow-wrap: break-word;
  text-align: left;
  font-size: 10px;
  color: #bdb8b8;
  width: 100%;
  display: flex;
  align-items: center;
}

.container-message-display .myself-message-body .message-text {
  background: #fff;
  padding: 7px 12px 8px 12px;
  line-height: 1.33;
  border-radius: 18px;
  margin-top: 1px;
  max-width: 100%;
  overflow-wrap: break-word;
  text-align: left;
  white-space: pre-wrap;
  /* border-bottom-right-radius: 4px; */
  word-break: break-word;
}
</style>